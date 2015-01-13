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
using EPlikt.Feed;

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
            var feedCreator = new LinqToXmlFeedCreator();
            feedCreator.SetFeedSource(new ChalmersFeedSource());

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(feedCreator.GetXmlFeedStr(), Encoding.UTF8, "application/rss+xml");

            return res;
        }
    }
}
