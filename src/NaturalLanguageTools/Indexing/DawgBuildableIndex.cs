using DocumentStorage;
using DawgSharp;

namespace NaturalLanguageTools.Indexing
{
    public class DawgBuildableIndex : IBuildableIndex<string>
    {
        private readonly DawgBuilder<RangePostingsList> builder;

        private readonly RangePostingsList allDocuments;

        public DawgBuildableIndex()
        {
            builder = new DawgBuilder<RangePostingsList>();
            allDocuments = new RangePostingsList();
        }

        public void IndexTerm(DocumentId id, string word, int position)
        {
            builder.TryGetValue(word, out var collectionList);

            if (collectionList == null)
            {
                collectionList = new RangePostingsList();
                builder.Insert(word, collectionList);
            }

            collectionList.Add(id);
            allDocuments.Add(id);
        }

        public DawgSearchableIndex Build()
        {
            return new DawgSearchableIndex(builder.BuildDawg(), allDocuments);
        }

        ISearchableIndex<string> IBuildableIndex<string>.Build()
        {
            return this.Build();
        }
    }
}
