using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPlikt.Feed
{
    public interface IFeedCreator
    {
        void SetFeedSource(IFeedSource src);
        string GetXmlFeedStr();
    }
}
