using Corpus;
using System.Collections.Generic;

namespace NaturalLanguageTools.Indexing
{
    public interface ISearchableIndex<T>
    {
        IEnumerable<DocumentId> Search(T word);

        int GetCount(T word);

        IEnumerable<DocumentId> GetAll();

        int GetCount();
    }
}
