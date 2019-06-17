using DocumentStorage;
using DawgSharp;

namespace NaturalLanguageTools.Indexing
{
    public class DawgBuildableIndex : IBuildableIndex<string>
    {
        private readonly DawgBuilder<DocumentIdRangeCollectionList> builder;

        private readonly DocumentIdRangeCollectionList allDocuments;

        public DawgBuildableIndex()
        {
            builder = new DawgBuilder<DocumentIdRangeCollectionList>();
            allDocuments = new DocumentIdRangeCollectionList();
        }

        public void IndexWord(DocumentId id, string word, int position)
        {
            builder.TryGetValue(word, out var collectionList);

            if (collectionList == null)
            {
                collectionList = new DocumentIdRangeCollectionList();
                builder.Insert(word, collectionList);
            }

            collectionList.Add(id);
            allDocuments.Add(id);
        }

        public DawgSearchableIndex CreateIndex()
        {
            return new DawgSearchableIndex(builder.BuildDawg(), allDocuments);
        }
    }
}
