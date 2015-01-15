using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPlikt.Feed
{
    /// <summary>
    /// Handles the feed source management of a feed creator.
    /// </summary>
    public abstract class FeedCreatorBase : IFeedCreator
    {
        protected IFeedSource feedSource;

        public void SetFeedSource(IFeedSource src)
        {
            feedSource = src;
        }

        public abstract void CreateFeed();
        public abstract int GetItemsCount();
        public abstract string GetXmlFeedStr();
    }
}