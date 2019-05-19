using System.Collections.Generic;

namespace Wikidump
{
    public interface IWikiDumpWriter
    {
        void WritePages(IEnumerable<WikiPage> pages);
    }
}
