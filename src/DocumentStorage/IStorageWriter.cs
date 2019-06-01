using System.Collections.Generic;

namespace DocumentStorage
{
    /// <summary>
    /// Document Storage writer interface
    /// </summary>
    public interface IStorageWriter<T>
    {
        void Write(IEnumerable<DocumentCollection<T>> wikipedia);
    }
}
