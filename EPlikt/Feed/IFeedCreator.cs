using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPlikt.Feed
{
    /// <summary>
    /// Creates an RSS feed with e-plikt specific content.
    /// </summary>
    public interface IFeedCreator
    {
        /// <summary>
        /// Sets the current source which will be used to create content for the feed.
        /// </summary>
        /// <param name="src">The source which will deliver our e-plikt specific content to us.</param>
        void SetFeedSource(IFeedSource src);

        /// <summary>
        /// Retrieves a string with the XML data for the feed, created from the current feed source.
        /// </summary>
        /// <returns>String with XML data representing the feed.</returns>
        string GetXmlFeedStr();
    }
}
