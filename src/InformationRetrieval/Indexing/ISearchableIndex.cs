using Corpus;
using System.Collections.Generic;

namespace InformationRetrieval.Indexing
{
    public interface ISearchableIndex<T>
    {
        IEnumerable<DocumentId> Search(T word);

        int GetCount(T word);

        IEnumerable<DocumentId> GetAll();

        int GetCount();
    }
}
