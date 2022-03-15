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
using System.Text;

namespace EPlikt.Feed
{
    public class ChalmersFeedSource : EPliktFeedSource
    {
        public static string ApiBaseUrl = ConfigurationManager.AppSettings["webapiBaseURL"];
        public static string ApiPublicationBaseUrl = ConfigurationManager.AppSettings["webapiChalmersResearchPublicationsBaseURL"];
        public static string ApiPublicationUserId = ConfigurationManager.AppSettings["ResearchRemoteBasicAuthUsername"];
        public static string ApiPublicationPw = ConfigurationManager.AppSettings["ResearchRemoteBasicAuthPassword"];
        public static string researchBaseUrl = ConfigurationManager.AppSettings["ResearchBaseUrl"];
        public static string feedLink = ConfigurationManager.AppSettings["feedLink"];

        /// <summary>
        /// Authentication credentials to the WebApi
        /// </summary>
        public static NetworkCredential NetworkCredentials = new NetworkCredential(ApiPublicationUserId, ApiPublicationPw);

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
            feed.Link = feedLink;
            feed.Language = "sv";
            feed.Copyright = "Chalmers Tekniska Högskola 2015-";
            feed.Description = "Material från Chalmers Tekniska Högskola som faller under lagen om leveransplikt för elektroniskt material.";
            feed.Image.Title = "Chalmers Tekniska Högskola - Pliktleverans av elektroniskt material";
            feed.Image.Url = researchBaseUrl + "/Images/chalmers_bldmrk.jpg";
            feed.Image.Link = feedLink;
            feed.Image.Width = "86";
            feed.Image.Height = "81";
            feed.Image.Description = "Chalmers tekniska högskola";
        }

        private void FillInDataForFeedItems(EPliktFeedContent feed, dynamic records)
        {
           foreach (var doc in records.Publications)
            {
                String pubtype = String.Empty;                
                String abstractp = String.Empty;
                String keywords = String.Empty;
                String pubdateRfc822 = String.Empty;
                String pdate = String.Empty;
                String ptitle = String.Empty;

                if (doc["Title"] != null)
                {
                    ptitle = doc["Title"];
                    ptitle = ptitle.Trim();
                }

                if (doc["PublicationType"] != null)
                {
                    pubtype = doc["PublicationType"]["NameSwe"];
                }

                if (doc["Abstract"] != null)
                {
                    abstractp = doc["Abstract"];
                    abstractp = abstractp.Trim();
                    abstractp = HttpUtility.HtmlDecode(abstractp.ToString());
                    abstractp = abstractp.Replace("\n", " ").Replace("\r", " ");
                }

                List<string> creators = new List<string>();

                if (doc["Persons"] != null)
                {
                    foreach (var person in doc["Persons"])
                    {
                        String author = String.Empty;
                        String role = String.Empty;

                        if (person.Role != null)
                        {
                            role = person.Role["NameSwe"];
                        }
                        else
                        {
                            role = "Författare";
                        }

                        if (person.PersonData != null)
                        {
                            author = person.PersonData["LastName"] + ", " + person.PersonData["FirstName"] + " (" + role;
                            creators.Add(author.Trim());
                        }
                    }
                }

                //if (!String.IsNullOrEmpty((String)doc["LatestEventDate"]))
                //{
                //    DateTime pubdate;
                //    pdate = ((String)doc["LatestEventDate"]);

                //    if (DateTime.TryParse(pdate, out pubdate))
                //    {
                //        pubdateRfc822 = pubdate.ToString("ddd, dd MMM yyyy HH:mm:ss +0100", CultureInfo.InvariantCulture);
                //    }
                //}
                //else
                //{
                //    DateTime pubdate;
                //    pdate = ((String)doc["CreatedDate"]);

                //    if (DateTime.TryParse(pdate, out pubdate))
                //    {
                //        pubdateRfc822 = pubdate.ToString("ddd, dd MMM yyyy HH:mm:ss +0100", CultureInfo.InvariantCulture);
                //    }
                //}

                if (doc["Keywords"] != null)
                {
                    var index = 0;
                    StringBuilder sb = new StringBuilder();
                    foreach (var keyword in doc["Keywords"])
                    {
                        string kw = keyword["Value"];
                        kw = kw.Trim();
                        if (kw.EndsWith(","))
                        {
                            kw = kw.Remove(kw.Length - 1);
                        }
                        sb.Append(kw);
                        if (index > 0)
                        {
                            sb.Append(", ");
                        }
                        index++;
                    }
                    keywords = sb.ToString().Trim();
                    // Remove last , if exists
                    if (keywords.EndsWith(","))
                    {
                        keywords = keywords.Remove(keywords.Length - 1);
                    }
                }

                if (doc["DataObjects"] != null)
                {

                    foreach (var fulltext in doc.DataObjects)
                    {
                        String url = String.Empty;
                        String md5Sum = String.Empty;
                        String mimetype = String.Empty;
                        
                        DateTime? embargodate;

                        if (fulltext["EmbargoDate"] != null)
                        {
                            DateTime embargodate_notnull;
                            string edate = ((String)fulltext["EmbargoDate"]);
                            if (DateTime.TryParse(edate, out embargodate_notnull))
                            {
                                embargodate = embargodate_notnull;
                            }
                            else
                            {
                                embargodate = null;
                            }
                        }
                        else
                        {
                            embargodate = null;
                        }
                        
                        if (fulltext["IsLocal"] == true && fulltext["IsMainFulltext"] == true && url != null && (embargodate == null || embargodate < DateTime.Now))
                        {
                            if (fulltext["Url"] != null)
                            {
                                url = (String)fulltext["Url"];

                                if (!url.StartsWith("http"))
                                {
                                    url = "https://research.chalmers.se/" + url;
                                }
                            }

                            if (fulltext["CreatedDate"] != null)
                            {
                                pdate = (String)fulltext["CreatedDate"];
                            }
                            else {
                                pdate = (String)doc["LatestEventDate"];
                            }

                            DateTime pubdate;

                            if (DateTime.TryParse(pdate, out pubdate))
                            {
                                pubdateRfc822 = pubdate.ToString("ddd, dd MMM yyyy HH:mm:ss +0100", CultureInfo.InvariantCulture);
                            }

                            if (fulltext["MimeType"] != null)
                            {
                                mimetype = (String)fulltext["MimeType"];
                            }

                            if (fulltext["Md5Sum"] != null)
                            {
                                md5Sum = (String)fulltext["Md5Sum"];
                            }

                            var item = new EPliktFeedItem();
                            item.Guid = url;
                            item.Title = ptitle;
                            item.Abstract = abstractp;
                            item.Keywords = keywords;
                            item.Category = pubtype;
                            item.Link = url;
                            item.PubDate = pubdateRfc822;
                            item.Publisher = publisher;
                            item.AccessRights = free;
                            item.ContentType = mimetype;
                            item.MD5 = md5Sum;                          
                            item.Creator = creators;

                            feed.Items.Add(item);
                        }
                    }
                }
            }
        }

        private dynamic GetAllRecords()
        {
            string jsonPublications = null;

            //string query = "_exists_:DataObjects and _exists_:ValidatedBy and _exists_:LatestEventDate and IsDeleted:false and IsDraft:false and DataObjects.MimeType:application/pdf and DataObjects.IsOpenAccess:true and Year:[2015 TO *] and ValidatedDate:[* TO now-90d]";
            string query = "_exists_:DataObjects and _exists_:ValidatedBy and _exists_:LatestEventDate and IsDeleted:false and IsDraft:false and DataObjects.MimeType:application/pdf and DataObjects.IsOpenAccess:true and DataObjects.IsLocal:true and Year:[2015 TO *] and DataObjects.CreatedDate:[now-8d TO now]";
            // Full dump:
            //string query = "_exists_:DataObjects and _exists_:ValidatedBy and _exists_:LatestEventDate and IsDeleted:false and IsDraft:false and DataObjects.MimeType:application/pdf and DataObjects.IsOpenAccess:true and DataObjects.IsLocal:true and Year:[2015 TO *]";
            String queryEnc = HttpUtility.UrlEncode(query);

            jsonPublications = (GetPublications(queryEnc,
            25000,
            0,
            "DataObjects.CreatedDate",
            "desc",
            null,
            new string[] {
                "Id",
                "Year",
                "Title",
                "Abstract",
                "CreatedDate",
                "UpdatedDate",
                "ValidatedDate",
                "LatestEventDate",
                "PublicationType.NameSwe",
                "Persons.PersonData.FirstName",
                "Persons.PersonData.LastName",
                "Persons.Role.NameSwe",
                "Keywords",
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

        private string GetPublications(string query, int max, int start, string sort, string sortOrder, string sortMultiple, string[] selectedFields)
        {
            string sf = selectedFields == null ? null : "&selectedFields=" + string.Join(",", selectedFields);
            return DownloadPublicationApiDataString("/Publications?query=" + query + "&max=" + max + "&sort=" + sort + sf + "&start=" + start + "&sortOrder=" + sortOrder);
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
