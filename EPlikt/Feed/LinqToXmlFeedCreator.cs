using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using EPlikt.IO;
using EPlikt.Models;
using EPlikt.Extensions;

namespace EPlikt.Feed
{
    public class LinqToXmlFeedCreator : FeedCreatorBase
    {
        protected XNamespace georss;
        protected XNamespace media;
        protected XNamespace dcterms;

        public LinqToXmlFeedCreator()
        {
            georss = ConfigurationManager.AppSettings["GeoRssNs"].ToString();
            media = ConfigurationManager.AppSettings["MediaNs"].ToString();
            dcterms = ConfigurationManager.AppSettings["DCtermsNs"].ToString();
        }

        override public string GetXmlFeedStr()
        {
            var content = feedSource.GetContent();

            var channel = CreateChannelElementFromModel(content);

            AddItemElementsToParentFromModel(channel, content);

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XAttribute(XNamespace.Xmlns + "georss", ConfigurationManager.AppSettings["GeoRssNs"].ToString()),
                    new XAttribute(XNamespace.Xmlns + "media", ConfigurationManager.AppSettings["MediaNs"].ToString()),
                    new XAttribute(XNamespace.Xmlns + "dcterms", ConfigurationManager.AppSettings["DCtermsNs"].ToString()),
                    channel
                )
            );

            var feedStr = "";
            using (var o = new Utf8StringWriter())
            using (var w = new XmlTextWriter(o))
            {
                doc.Save(w);
                feedStr = o.ToString();
            }

            return feedStr;
        }

        private XElement CreateChannelElementFromModel(EPliktFeedContent model)
        {
            return new XElement("channel",
                new XElement("title", model.Title),
                new XElement("link", model.Link),
                new XElement("language", model.Language),
                new XElement("copyright", model.Copyright),
                new XElement("description", model.Description),
                new XElement("image",
                    new XElement("title", model.Image.Title),
                    new XElement("url", model.Image.Url),
                    new XElement("link", model.Image.Link),
                    new XElement("width", model.Image.Width),
                    new XElement("height", model.Image.Height),
                    new XElement("description", model.Image.Description)
                )
            );
        }

        private void AddItemElementsToParentFromModel(XElement parent, EPliktFeedContent model)
        {
            foreach (var item in model.Items)
            {
                // Clean potentially invalid XML chars in applicable fields
                string cleanTitle = item.Title.CleanInvalidXmlChars();

                // Correct capitalization of pubDate
                string pubdateUc = item.PubDate.UppercaseFirstEach();

                // Fields that should never be NULL or repeated
                var rss_item = new XElement("item",
                        new XElement("guid", item.Guid),
                        new XElement("pubDate", pubdateUc),
                        new XElement("title", cleanTitle),
                        new XElement("link", item.Link),
                        new XElement("category", item.Category),
                        new XElement(dcterms + "publisher", item.Publisher),
                        new XElement(dcterms + "format", item.ContentType),
                    //new XElement(dcterms + "MD5", item.MD5),
                        new XElement(dcterms + "accessRights", item.AccessRights)
                        );

                // Repeatable field(s)
                foreach (string creator in item.Creator)
                {
                    string cleanCreator = creator.CleanInvalidXmlChars();
                    // split name and role
                    string sCreator = cleanCreator.Replace(":", " (");
                    string goodCreator = sCreator + ")";
                    rss_item.Add(new XElement(dcterms + "creator", goodCreator));
                }

                // Fields that could be null
                if (!string.IsNullOrEmpty(item.Abstract))
                {
                    // remove invalid XML characters
                    string cleanAbstract = item.Abstract.CleanInvalidXmlChars();
                    rss_item.Add(new XElement("description", new XCData(cleanAbstract)));
                }

                if (!string.IsNullOrEmpty(item.Keywords))
                {
                    // remove invalid XML characters
                    string cleanKeywords = item.Keywords.CleanInvalidXmlChars();
                    rss_item.Add(new XElement(media + "keywords", cleanKeywords));
                }

                // add item to channel
                parent.Add(rss_item);
            }
        }
    }
}