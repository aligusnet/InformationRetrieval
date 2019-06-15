using DocumentStorage;

namespace NaturalLanguageTools.Indexing
{
    public interface IBuildableIndex<T>
    {
        void IndexWord(DocumentId id, T word, int position);
    }
}
