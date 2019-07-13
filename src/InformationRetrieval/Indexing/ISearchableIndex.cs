using Corpus;
using System.Collections.Generic;

namespace InformationRetrieval.Indexing
{
    public interface ISearchableIndex<T>
    {
        IReadOnlyCollection<DocumentId> Search(T word);

        int GetCount(T word);

        IReadOnlyCollection<DocumentId> GetAll();

        int GetCount();
    }
}
