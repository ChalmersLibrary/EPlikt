using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            XNamespace georss = "http://www.georss.org/georss";
            XNamespace media = "http://search.yahoo.com/mrss/";
            XNamespace dcterms = "http://purl.org/dc/terms/";

            var channel = new XElement("channel",
                new XElement("title", "Chalmers Tekniska Högskola - Pliktleverans av elektroniskt material"),
                new XElement("link", ""),
                new XElement("language", "sv"),
                new XElement("copyright", "Chalmers Tekniska Högskola 2015"),
                new XElement("description", "Material från Chalmers Tekniska Högskola som faller under EPliktslagen."),
                new XElement("image",
                    new XElement("title", "Chalmers Tekniska Högskola"),
                    new XElement("url", ""),
                    new XElement("link", ""),
                    new XElement("width", ""),
                    new XElement("height", ""),
                    new XElement("description", "MISSING IMAGE!")
                )
            );

            foreach (var item in content.Items)
            {
                channel.Add(new XElement("item", 
                        new XElement("title", item.Title),
                        new XElement("description", item.Abstract),
                        new XElement(media + "content", "")
                    ));
            }

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XAttribute(XNamespace.Xmlns + "georss", "http://www.georss.org/georss"),
                    new XAttribute(XNamespace.Xmlns + "media", "http://search.yahoo.com/mrss/"),
                    new XAttribute(XNamespace.Xmlns + "dcterms", "http://purl.org/dc/terms/"),
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
    }
}