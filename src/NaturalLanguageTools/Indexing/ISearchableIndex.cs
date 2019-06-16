using DocumentStorage;
using System.Collections.Generic;

namespace NaturalLanguageTools.Indexing
{
    public interface ISearchableIndex<T>
    {
        IEnumerable<DocumentId> Search(T word);
    }
}
