using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPlikt.Models
{
    public class EPliktFeedItem
    {
        public string Guid { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Link { get; set; }
        public string PubDate { get; set; }
        public string Publisher { get; set; }
        public string AccessRights { get; set; }
        public string License { get; set; }
        public string Format { get; set; }
        public string ContentType { get; set; }
        public string References { get; set; }
        public string MD5 { get; set; }
        public string Keywords { get; set; }
        public string Category { get; set; }
        public string Copyright { get; set; }
        public List<string> Creator { get; set; }
    }
}