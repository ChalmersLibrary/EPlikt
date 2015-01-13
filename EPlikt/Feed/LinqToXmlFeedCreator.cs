using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace EPlikt.Feed
{
    public class LinqToXmlFeedCreator : FeedCreatorBase
    {
        override public string GetXmlFeedStr()
        {
            var content = feedSource.GetContent();

            XNamespace georss = ConfigurationManager.AppSettings["GeoRssNs"].ToString();
            XNamespace media = ConfigurationManager.AppSettings["MediaNs"].ToString();
            XNamespace dcterms = ConfigurationManager.AppSettings["DCtermsNs"].ToString();

            var channel = new XElement("channel",
                new XElement("title", "Chalmers Tekniska Högskola - Pliktleverans av elektroniskt material"),
                new XElement("link", ""),
                new XElement("language", "sv"),
                new XElement("copyright", "Chalmers Tekniska Högskola 2015-"),
                new XElement("description", "Material från Chalmers Tekniska Högskola som faller under lagen om leveransplikt för elektroniskt material."),
                new XElement("image",
                    new XElement("title", "Chalmers Tekniska Högskola"),
                    new XElement("url", "http://publications.lib.chalmers.se/local/img/chalmers_bldmrk.jpg"),
                    new XElement("link", "http://www.chalmers.se"),
                    new XElement("width", ""),
                    new XElement("height", ""),
                    new XElement("description", "Chalmers tekniska högskola")
                )
            );

            foreach (var item in content.Items)
            {
                // Clean potentially invalid XML chars in applicable fields
                string cleanTitle = CleanInvalidXmlChars(item.Title);
                
                // Fields that should never be NULL or repeated
                var rss_item = new XElement("item",
                        new XElement("guid", item.Guid),
                        new XElement("pubDate", item.PubDate),
                        new XElement("title", cleanTitle),
                        new XElement("link", item.Link),
                        new XElement("category", item.Category),
                        new XElement(dcterms + "publisher", item.Publisher),
                        new XElement(dcterms + "format", item.ContentType),
                        new XElement(dcterms + "MD5", item.MD5),
                        new XElement(dcterms + "accessRights", item.AccessRights)
                        );

                // Repeatable field(s)
                foreach (string creator in item.Creator)
                {
                    string cleanCreator = CleanInvalidXmlChars(creator);
                    // split name and role
                    string sCreator = cleanCreator.Replace(":", " (");
                    string goodCreator = sCreator + ")";
                    rss_item.Add(new XElement(dcterms + "creator", goodCreator));
                }

                // Fields that could be null
                if (!string.IsNullOrEmpty(item.Abstract))
                {
                    // remove invalid XML characters
                    string cleanAbstract = CleanInvalidXmlChars(item.Abstract);
                    rss_item.Add(new XElement("description", new XCData(cleanAbstract)));
                }
                else
                {
                    // do nothing
                }

                if (!string.IsNullOrEmpty(item.Keywords))
                {
                    // remove invalid XML characters
                    string cleanKeywords = CleanInvalidXmlChars(item.Keywords);
                    rss_item.Add(new XElement(media + "keywords", cleanKeywords));
                }
                else
                {
                    // do nothing
                }

                // add item to channel
                channel.Add(rss_item);
                
            }

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

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

        public static string CleanInvalidXmlChars(string text)
        {
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }
    }
}