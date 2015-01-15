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
    public class ChalmersFeedSource : EPliktFeedSource
    {
        override public EPliktFeedContent GetContent()
        {
            var content = new EPliktFeedContent();

            FillInDataAboutFeed(content);

            FillInDataForFeedItems(content, GetAllRecords());

            return content;
        }

        private void FillInDataAboutFeed(EPliktFeedContent feed)
        {
            feed.Title = "Chalmers Tekniska Högskola - Pliktleverans av elektroniskt material";
            feed.Link = "http://feeds.lib.chalmers.se/api/eplikt/";
            feed.Language = "sv";
            feed.Copyright = "Chalmers Tekniska Högskola 2015-";
            feed.Description = "Material från Chalmers Tekniska Högskola som faller under lagen om leveransplikt för elektroniskt material.";
            feed.Image.Title = "Chalmers Tekniska Högskola - Pliktleverans av elektroniskt material";
            feed.Image.Url = "http://publications.lib.chalmers.se/local/img/chalmers_bldmrk.jpg";
            feed.Image.Link = "http://feeds.lib.chalmers.se/api/eplikt/";
            feed.Image.Width = "86";
            feed.Image.Height = "81";
            feed.Image.Description = "Chalmers tekniska högskola";
        }

        private void FillInDataForFeedItems(EPliktFeedContent feed, dynamic records)
        {
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

                feed.Items.Add(item);
            }
        }

        private dynamic GetAllRecords()
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