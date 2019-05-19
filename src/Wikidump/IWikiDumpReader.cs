using System;
using System.Collections.Generic;

namespace Wikidump
{
    public interface IWikiDumpReader : IDisposable
    {
        IEnumerable<WikiPage> ReadPages();
    }
}
