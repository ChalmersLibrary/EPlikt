using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPlikt.Models
{
    public class EPliktFeedContent
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Language { get; set; }
        public string Copyright { get; set; }
        public string Description { get; set; }
        public FeedImage Image { get; set; }
        public List<EPliktFeedItem> Items { get; set; }

        public EPliktFeedContent()
        {
            Items = new List<EPliktFeedItem>();
        }
    }
}