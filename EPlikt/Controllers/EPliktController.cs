using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EPlikt.Models;
using EPlikt.Feed;
using log4net;
using log4net.Config;

namespace EPlikt.Controllers
{
    public class EPliktController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EPliktController));

        /// <summary>
        /// Get the main feed.
        /// </summary>
        /// <returns>The feed.</returns>
        [HttpGet]
        public HttpResponseMessage Feed()
        {
            log.Info("Processing feed request.");

            var res = Request.CreateResponse(HttpStatusCode.OK);

            try
            { 
                var feedCreator = new LinqToXmlFeedCreator();
                feedCreator.SetFeedSource(new ChalmersFeedSource());
                feedCreator.CreateFeed();
                res.Content = new StringContent(feedCreator.GetXmlFeedStr(), Encoding.UTF8, "application/rss+xml");
                log.Info("Successfully delivered " + feedCreator.GetItemsCount() + " items.");
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                res.Content = new StringContent("ERROR: " + e.Message, Encoding.UTF8, "text/plain");
            }

            return res;
        }
    }
}
