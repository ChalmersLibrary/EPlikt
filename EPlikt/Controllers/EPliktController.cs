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
using Newtonsoft.Json.Linq;
using System.Collections;

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

            // Retrieve items from Solr and return as feed items
            var records = GetAllRecords();

            XNamespace dcterms = XNamespace.Get("http://purl.org/dc/terms/"); 

            foreach (var doc in records.response.docs)
            {
                var si = new SyndicationItem(
                    (String)doc.title,
                    (String)doc["abstract"],
                    new Uri((String)doc.url)
                );

                si.Id = doc.url;
                si.PublishDate = doc.pubdate;

                si.ElementExtensions.Add(new XElement(dcterms + "accessRights", String.Empty, "Gratis"));
                
                ret.Add(si);
            }

            return ret;
        }

        private static dynamic GetAllRecords()
        {
            HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create("http://cpltest.lib.chalmers.se:8080/solr/cpl_plikt/select?q=*:*&wt=json");
            fileReq.CookieContainer = new CookieContainer();
            fileReq.AllowAutoRedirect = true;
            HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();
            var outputStream = fileResp.GetResponseStream();

            var sr = new StreamReader(outputStream);
            return JsonConvert.DeserializeObject(sr.ReadToEnd());
        }
    }
}
