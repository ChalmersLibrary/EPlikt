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
    public class LinqToXmlFeedCreator : EPliktFeedCreator
    {
        protected string xmlFeedStr = "";
        protected int itemCount = 0;

        override public void CreateFeed()
        {
            var content = feedSource.GetContent();

            var channel = CreateChannelElementFromModel(content);

            AddItemElementsToParentFromModel(channel, content);

            XDocument doc = new XDocument(
                new XDeclaration(xmlVersion, xmlEncoding, xmlStandalone),
                new XElement(rssXmlElementName,
                    new XAttribute(rssVersionAttributeName, rssVersion),
                    new XAttribute(XNamespace.Xmlns + georssNamespaceName, georssNamespace),
                    new XAttribute(XNamespace.Xmlns + mediaNamespaceName, mediaNamespace),
                    new XAttribute(XNamespace.Xmlns + dctermsNamespaceName, dctermsNamespace),
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

            xmlFeedStr = feedStr;
        }

        override public int GetItemsCount()
        {
            return itemCount;
        }

        override public string GetXmlFeedStr()
        {
            return xmlFeedStr;
        }

        private XElement CreateChannelElementFromModel(EPliktFeedContent model)
        {
            return new XElement(channelXmlElementName,
                new XElement(titleXmlElementName, model.Title),
                new XElement(linkXmlElementName, model.Link),
                new XElement(languageXmlElementName, model.Language),
                new XElement(copyrightXmlElementName, model.Copyright),
                new XElement(descriptionXmlElementName, model.Description),
                new XElement(imageXmlElementName,
                    new XElement(imageTitleXmlElementName, model.Image.Title),
                    new XElement(imageUrlXmlElementName, model.Image.Url),
                    new XElement(imageLinkXmlElementName, model.Image.Link),
                    new XElement(imageWidthXmlElementName, model.Image.Width),
                    new XElement(imageHeightXmlElementName, model.Image.Height),
                    new XElement(imageDescriptionXmlElementName, model.Image.Description)
                )
            );
        }

        private void AddItemElementsToParentFromModel(XElement parent, EPliktFeedContent model)
        {
            itemCount = 0;
            foreach (var item in model.Items)
            {
                // Clean potentially invalid XML chars in applicable fields
                string cleanTitle = item.Title.CleanInvalidXmlChars();

                // Correct capitalization of pubDate
                string pubdateUc = item.PubDate.UppercaseFirstEach();

                // Fields that should never be NULL or repeated
                var rss_item = new XElement(itemXmlElementName,
                        new XElement(itemGuidXmlElementName, item.Guid),
                        new XElement(itemPubDateXmlElementName, pubdateUc),
                        new XElement(itemTitleXmlElementName, cleanTitle),
                        new XElement(itemLinkXmlElementName, item.Link),
                        new XElement(itemCategoryXmlElementName, item.Category),
                        new XElement(dcterms + itemPublisherXmlElementName, item.Publisher),
                        new XElement(dcterms + itemFormatXmlElementName, item.ContentType),
                    // new XElement(dcterms + itemMd5XmlElementName, item.MD5),
                        new XElement(dcterms + itemAccessRightsXmlElementName, item.AccessRights)
                        );

                // Repeatable field(s)
                foreach (string creator in item.Creator)
                {
                    string cleanCreator = creator.CleanInvalidXmlChars();
                    // split name and role
                    string sCreator = cleanCreator.Replace(":", " (");
                    string goodCreator = sCreator + ")";
                    rss_item.Add(new XElement(dcterms + itemCreatorXmlElementName, goodCreator));
                }

                // Fields that could be null
                if (!string.IsNullOrEmpty(item.Abstract))
                {
                    // remove invalid XML characters
                    string cleanAbstract = item.Abstract.CleanInvalidXmlChars();
                    rss_item.Add(new XElement(itemDescriptionXmlElementName, new XCData(cleanAbstract)));
                }

                if (!string.IsNullOrEmpty(item.Keywords))
                {
                    // remove invalid XML characters
                    string cleanKeywords = item.Keywords.CleanInvalidXmlChars();
                    rss_item.Add(new XElement(media + itemKeywordsXmlElementName, cleanKeywords));
                }

                itemCount++;

                // add item to channel
                parent.Add(rss_item);
            }
        }
    }
}