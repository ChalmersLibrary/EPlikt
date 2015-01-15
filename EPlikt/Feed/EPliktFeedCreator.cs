using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace EPlikt.Feed
{
    /// <summary>
    /// Holds e-plikt protocol specific data. 
    /// http://www.kb.se/namespace/digark/deliveryspecification/deposit/rssfeeds/rssfeeds.pdf
    /// </summary>
    public abstract class EPliktFeedCreator : FeedCreatorBase
    {
        protected const string xmlVersion = "1.0";
        protected const string xmlEncoding = "utf-8";
        protected const string xmlStandalone = "no";

        protected const string georssNamespaceName = "georss";
        protected const string mediaNamespaceName = "media";
        protected const string dctermsNamespaceName = "dcterms";

        protected const string georssNamespace = "http://www.georss.org/georss";
        protected const string dctermsNamespace = "http://purl.org/dc/terms/";
        protected const string mediaNamespace = "http://search.yahoo.com/mrss/";

        protected XNamespace georss;
        protected XNamespace media;
        protected XNamespace dcterms;

        protected const string rssXmlElementName = "rss";
        protected const string rssVersionAttributeName = "version";
        protected const string rssVersion = "2.0";

        protected const string channelXmlElementName = "channel";
        protected const string titleXmlElementName = "title";
        protected const string linkXmlElementName = "link";
        protected const string languageXmlElementName = "language";
        protected const string copyrightXmlElementName = "copyright";
        protected const string descriptionXmlElementName = "description";
        protected const string imageXmlElementName = "image";
        protected const string imageTitleXmlElementName = "title";
        protected const string imageUrlXmlElementName = "url";
        protected const string imageLinkXmlElementName = "link";
        protected const string imageWidthXmlElementName = "width";
        protected const string imageHeightXmlElementName = "height";
        protected const string imageDescriptionXmlElementName = "description";

        protected const string itemXmlElementName = "item";
        protected const string itemGuidXmlElementName = "guid";
        protected const string itemPubDateXmlElementName = "pubDate";
        protected const string itemTitleXmlElementName = "title";
        protected const string itemLinkXmlElementName = "link";
        protected const string itemCategoryXmlElementName = "category";
        protected const string itemPublisherXmlElementName = "publisher";
        protected const string itemFormatXmlElementName = "format";
        protected const string itemMd5XmlElementName = "MD5";
        protected const string itemAccessRightsXmlElementName = "accessRights";
        protected const string itemCreatorXmlElementName = "creator";
        protected const string itemDescriptionXmlElementName = "description";
        protected const string itemKeywordsXmlElementName = "keywords";

        public EPliktFeedCreator()
        {
            georss = georssNamespace;
            media = mediaNamespace;
            dcterms = dctermsNamespace;
        }
    }
}