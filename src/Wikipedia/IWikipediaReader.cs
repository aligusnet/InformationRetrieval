using System.Collections.Generic;

namespace Wikipedia
{
    /// <summary>
    /// Wikipedia's Storage reader interface
    /// </summary>
    public interface IWikipediaReader
    {
        IEnumerable<WikiCollection> Read(string path);
    }
}
