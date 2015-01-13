using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPlikt.Models
{
    public class EPliktFeedContent
    {
        public List<EPliktFeedItem> Items { get; set; }

        public EPliktFeedContent()
        {
            Items = new List<EPliktFeedItem>();
        }
    }
}