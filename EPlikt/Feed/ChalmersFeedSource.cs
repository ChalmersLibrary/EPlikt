using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using EPlikt.Models;

namespace EPlikt.Feed
{
    public class ChalmersFeedSource : IFeedSource
    {
        public EPliktFeedContent GetContent()
        {
            var content = new EPliktFeedContent();

            var records = GetAllRecords();

            foreach (var doc in records.response.docs)
            {
                var item = new EPliktFeedItem();
                item.Title = (String)doc.title;
                item.Abstract = (String)doc["abstract"];
                content.Items.Add(item);

                // TODO: Fill in more data.
            }

            return content;
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