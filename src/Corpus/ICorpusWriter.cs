using System.Collections.Generic;

namespace Corpus
{
    /// <summary>
    /// Document Corpus writer interface
    /// </summary>
    public interface ICorpusWriter<T>
    {
        void Write(IEnumerable<Block<T>> wikipedia);
    }
}
