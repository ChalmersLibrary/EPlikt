using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ComponentModel.DataAnnotations;
using EPlikt.Models;
using Newtonsoft.Json;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace EPlikt.Controllers
{
    public class EPliktController : ApiController
    {
        /// <summary>
        /// Get the main feed.
        /// </summary>
        /// <returns>The feed.</returns>
        [HttpGet]
        public HttpResponseMessage Feed()
        {
            var feed = new SyndicationFeed(GetSyndicationItems())
            {
                Title = new TextSyndicationContent("Innehåll på Chalmers"),
                Description = new TextSyndicationContent("Denna feed innehåller data från Chalmers som berörs utav den nya E-Pliktslagen.")
            };
            
            // Create a new  XmlDocument in order to modify the root element
            // http://stackoverflow.com/questions/14397392/overwriting-root-element-in-syndicationfeed-adding-namespaces-to-root-element
            var rssFeedFormatter = new Rss20FeedFormatter(feed);
            var xmlDoc = new XmlDocument();

            // Write the RSS formatted feed directly into the xml doc
            using (var xw = xmlDoc.CreateNavigator().AppendChild())
            {
                rssFeedFormatter.WriteTo(xw);
            }

            // Add custom namespace(s)
            xmlDoc.DocumentElement.SetAttribute("xmlns:georss", "http://www.georss.org/georss");
            xmlDoc.DocumentElement.SetAttribute("xmlns:media", "http://search.yahoo.com/mrss/");
            xmlDoc.DocumentElement.SetAttribute("xmlns:dcterms", "http://purl.org/dc/terms/");

            var output = new StringWriter();
            var writer = new XmlTextWriter(output);

            //new Rss20FeedFormatter(feed).WriteTo(writer);
            xmlDoc.WriteTo(writer);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(output.ToString(), Encoding.UTF8, "application/rss+xml");

            return res;
        }

        private static List<SyndicationItem> GetSyndicationItems()
        {
            var ret = new List<SyndicationItem>();

            ret.Add(new SyndicationItem("TestTitle1", "TestContent1", new Uri("http://www.google.com")));
            ret.Add(new SyndicationItem("TestTitle2", "TestContent2", new Uri("http://www.google.com")));
            ret.Add(new SyndicationItem("TestTitle3", "TestContent3", new Uri("http://www.google.com")));

            return ret;
        }
    }

    
}
