using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
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

            // Static variables (defaults)
            string publisher = "http://id.kb.se/organisations/SE5564795598";
            string free = "gratis";

            foreach (var doc in records.response.docs)
            {
                var item = new EPliktFeedItem();
                item.Guid = (String)doc["url"];
                item.Title = (String)doc.title;
                item.Abstract = (String)doc["abstract"];
                item.Keywords = (String)doc["keywords"];
                item.Category = (String)doc["pubtype"];
                item.Link = (String)doc["url"];
                item.PubDate = (String)doc["pubdate_rfc822"];
                item.Publisher = publisher;
                item.AccessRights = free;
                item.ContentType = (String)doc["mimetype"];
                item.MD5 = (String)doc["md5sum"];
                List<string> creators = doc["person_role_mapping"].ToObject<List<string>>();
                item.Creator = creators;
                
                content.Items.Add(item);
            }

            
            
            return content;
        }

        private static dynamic GetAllRecords()
        {
            string SolrUrl = ConfigurationManager.AppSettings["SolrUrl"].ToString();

            HttpWebRequest fileReq = (HttpWebRequest)HttpWebRequest.Create(SolrUrl + "select?q=*:*&wt=json&rows=9999");
            fileReq.CookieContainer = new CookieContainer();
            fileReq.AllowAutoRedirect = true;
            HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();
            var outputStream = fileResp.GetResponseStream();

            var sr = new StreamReader(outputStream);
            return JsonConvert.DeserializeObject(sr.ReadToEnd());
        }
    }

    
}