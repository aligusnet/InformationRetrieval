using System.Collections.Generic;

namespace DocumentStorage
{
    /// <summary>
    /// Document Storage reader interface
    /// </summary>
    public interface IStorageReader<T>
    {
        IEnumerable<DocumentCollection<T>> Read();
    }
}
