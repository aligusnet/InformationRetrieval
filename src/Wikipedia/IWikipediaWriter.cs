using System.Collections.Generic;

namespace Wikipedia
{
    /// <summary>
    /// Wikipedia's Storage writer interface
    /// </summary>
    public interface IWikipediaWriter
    {
        void Write(IEnumerable<WikiCollection> wikipedia, string path);
    }
}
