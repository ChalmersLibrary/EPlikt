using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPlikt.Models;

namespace EPlikt.Feed
{
    /// <summary>
    /// A source to a feed which handles e-plikt specific content.
    /// </summary>
    public interface IFeedSource
    {
        /// <summary>
        /// Retrieves the content and exposes it through an e-plikt specific model. 
        /// </summary>
        /// <returns>The populated feed content model.</returns>
        EPliktFeedContent GetContent();
    }
}
