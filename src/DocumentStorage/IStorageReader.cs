using System.Collections.Generic;

namespace DocumentStorage
{
    /// <summary>
    /// Document Storage reader interface
    /// </summary>
    public interface IStorageReader
    {
        IEnumerable<DocumentCollection> Read();
    }
}
