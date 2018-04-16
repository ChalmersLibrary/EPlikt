using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using EPlikt.Models;
using System.Globalization;

namespace EPlikt.Feed
{
    public class ChalmersFeedSource : EPliktFeedSource
    {
        public static string ApiBaseUrl = ConfigurationManager.AppSettings["webapiBaseURL"];
        public static string ApiPublicationBaseUrl = ConfigurationManager.AppSettings["webapiChalmersResearchPublicationsBaseURL"];

        /// <summary>
        /// Authentication credentials to the WebApi
        /// </summary>
        public static NetworkCredential NetworkCredentials = new NetworkCredential("research", "research99");

        override public EPliktFeedContent GetContent()
        {
            var content = new EPliktFeedContent();

            FillInDataAboutFeed(content);

            //FillInDataForFeedItems(content, GetAllRecords());
            FillInDataForFeedItems(content, GetAllResearchRecords());

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
            feed.Image.Url = "https://research.chalmers.se/Images/chalmers-avancez.png";
            feed.Image.Link = "http://feeds.lib.chalmers.se/api/eplikt/";
            feed.Image.Width = "86";
            feed.Image.Height = "81";
            feed.Image.Description = "Chalmers tekniska högskola";
        }

        //private void FillInDataForFeedItems(EPliktFeedContent feed, dynamic records)
        //{
        //    foreach (var doc in records.response.docs)
        //    {
        //        var item = new EPliktFeedItem();
        //        item.Guid = (String)doc["url"];
        //        item.Title = (String)doc.title;
        //        item.Abstract = (String)doc["abstract"];
        //        item.Keywords = (String)doc["keywords"];
        //        item.Category = (String)doc["pubtype"];
        //        item.Link = (String)doc["url"];
        //        item.PubDate = (String)doc["pubdate_rfc822"];
        //        item.Publisher = publisher;
        //        item.AccessRights = free;
        //        item.ContentType = (String)doc["mimetype"];
        //        item.MD5 = (String)doc["md5sum"];
        //        List<string> creators = doc["person_role_mapping"].ToObject<List<string>>();
        //        item.Creator = creators;

        //        feed.Items.Add(item);
        //    }
        //}

        private void FillInDataForFeedItems(EPliktFeedContent feed, dynamic records)
        {
            foreach (var doc in records.Publications)
            {
                String pubtype = String.Empty;
                String pubdateRfc822 = String.Empty;
                String url = String.Empty;

                if (!String.IsNullOrEmpty((String)doc["ModifiedDate"]))
                {
                    DateTime pubdate = new DateTime(doc["ModifiedDate"]);
                    pubdateRfc822 = pubdate.ToString("ddd, dd MMM yyyy HH:mm:ss Z", CultureInfo.InvariantCulture);
                }
                else
                {
                    DateTime pubdate = new DateTime(doc["CreatedDate"]);
                    pubdateRfc822 = pubdate.ToString("ddd, dd MMM yyyy HH:mm:ss Z", CultureInfo.InvariantCulture);
                }

                if (doc["IdentifierCplPubid"] != null)
                {
                    string pubid = doc["IdentifierCplPubid"][0];
                    url = "...." + pubid;
                }

                var item = new EPliktFeedItem();
                item.Guid = (String)url;
                item.Title = (String)doc["Title"];
                item.Abstract = (String)doc["Abstract"];
                //item.Keywords = (String)doc["keywords"];
                item.Category = (String)doc["pubtype"];
                item.Link = (String)url;
                item.PubDate = (String)pubdateRfc822;
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

        private dynamic GetAllResearchRecords()
        {
            string jsonPublications = null;

            string query = "_exists_%3ADataObjects%20and%20_exists_%3AValidatedBy%20and%20IsDeleted%3Afalse%20and%20IsDraft%3Afalse%20and%20DataObjects.MimeType%3Aapplication%2Fpdf%20and%20DataObjects.IsLocal%3Atrue%20and%20DataObjects.IsMainFulltext%3Atrue%20and%20DataObjects.AccessType%3A3%20and%20Year%3A%5B2015%20TO%20*%5D%20and%20CreatedDate%3A%5B*%20TO%20now-90d%5D%20and%20DataObjects.EmbargoDate%3A%5B*%20TO%20now%5D";
            
            jsonPublications = (GetPublications(query,
            10000,
            0,
            "CreatedDate",
            "desc",
            new string[] {
                "Id",
                "Year",
                "IdentifierCplPubid",
                "Title",
                "Abstract",
                "CreatedDate",
                "ModifiedDate",
                "Persons.DisplayName",
                "DataObjects"}));

            if (jsonPublications != null)
            {
                return JsonConvert.DeserializeObject(jsonPublications);
            }
            else
            {
                return null;
            }            
        }

        private string GetPublications(string query, int max, int start, string sort, string sortOrder, string[] selectedFields)
        {
            string sf = selectedFields == null ? null : "&selectedFields=" + string.Join(",", selectedFields);
            return DownloadPublicationApiDataString("/publication?query={query}max={max}&sort={sort}{sf}&start={start}&sortOrder={sortOrder}");
        }

        public static string DownloadPublicationApiDataString(string apiEndPoint)
        {       
            string apiUrl = ApiPublicationBaseUrl + apiEndPoint;

            using (WebClient web = new WebClient())
            {
                web.Encoding = System.Text.Encoding.UTF8;

                if (NetworkCredentials != null)
                {
                    web.Credentials = NetworkCredentials;
                }

                string data;

                try
                {
                    data = web.DownloadString(apiUrl);
                }
                catch (Exception e)
                {
                    data = "{\"status\":\"error\", \"endpoint\": \"" + apiUrl + "\", \"message\":\"" + e.Message + "\"}";
                }

                return data;
            }
        }


    }

    
}