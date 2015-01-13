using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPlikt.Feed
{
    public abstract class FeedCreatorBase : IFeedCreator
    {
        protected IFeedSource feedSource;

        public void SetFeedSource(IFeedSource src)
        {
            feedSource = src;
        }

        abstract public string GetXmlFeedStr();
    }
}