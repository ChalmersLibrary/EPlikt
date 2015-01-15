using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPlikt.Models;

namespace EPlikt.Feed
{
    /// <summary>
    /// Holds e-plikt protocol specific default values.
    /// http://www.kb.se/namespace/digark/deliveryspecification/deposit/rssfeeds/rssfeeds.pdf
    /// </summary>
    public abstract class EPliktFeedSource : IFeedSource
    {
        protected const string publisher = "http://id.kb.se/organisations/SE5564795598";
        protected const string free = "gratis";

        public abstract EPliktFeedContent GetContent();
    }
}