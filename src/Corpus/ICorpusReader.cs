using System.Collections.Generic;

namespace Corpus
{
    /// <summary>
    /// Document Corpus reader interface
    /// </summary>
    public interface ICorpusReader<T>
    {
        IEnumerable<Block<T>> Read();
    }
}
