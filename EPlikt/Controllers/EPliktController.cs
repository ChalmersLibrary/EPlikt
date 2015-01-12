using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ComponentModel.DataAnnotations;
using EPlikt.Models;
using System.ServiceModel.Syndication;
using System.Xml;
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

            var output = new StringWriter();
            var writer = new XmlTextWriter(output);

            new Rss20FeedFormatter(feed).WriteTo(writer);

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
