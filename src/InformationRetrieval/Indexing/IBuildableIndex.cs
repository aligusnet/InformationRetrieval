using Corpus;

namespace InformationRetrieval.Indexing
{
    public interface IBuildableIndex<T> where T : notnull
    {
        void IndexTerm(DocumentId id, T term, int position);

        ISearchableIndex<T> Build();
    }
}
