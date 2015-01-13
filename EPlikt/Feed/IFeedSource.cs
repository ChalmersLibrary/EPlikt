using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPlikt.Models;

namespace EPlikt.Feed
{
    public interface IFeedSource
    {
        EPliktFeedContent GetContent();
    }
}
