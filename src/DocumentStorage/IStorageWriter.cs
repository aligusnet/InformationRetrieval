using System.Collections.Generic;

namespace DocumentStorage
{
    /// <summary>
    /// Document Storage writer interface
    /// </summary>
    public interface IStorageWriter
    {
        void Write(IEnumerable<DocumentCollection> wikipedia);
    }
}
