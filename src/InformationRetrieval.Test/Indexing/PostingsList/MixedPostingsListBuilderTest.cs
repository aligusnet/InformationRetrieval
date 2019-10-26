using System;
using System.Collections.Generic;

using Corpus;
using InformationRetrieval.Indexing;
using InformationRetrieval.Indexing.PostingsList;

namespace InformationRetrieval.Test.Indexing.PostingsList
{
    public class MixedPostingsListBuilderTest : IndexUnitTestsBase<MixedPostingsListBuilderTest.Index>
    {
        private const int rangeThreshold = 3;

        protected override Index CreateIndex(string[][] corpus)
        {
            var index = new Index();
            IndexHelper.BuildIndex(index, corpus);
            return index;
        }

        public class Index : IBuildableIndex<string>, ISearchableIndex<string>
        {
            MixedPostingsListBuilder<string> builder = new MixedPostingsListBuilder<string>(rangeThreshold);

            public ISearchableIndex<string> Build()
            {
                return this;
            }

            public IReadOnlyCollection<DocumentId> GetAll()
            {
                return builder.AllDocuments;
            }

            public int GetCount(string word)
            {
                if (builder.RangedPostingsLists.TryGetValue(word, out var blockList))
                {
                    return blockList.Count;
                }

                if (builder.UncompressedPostingsLists.TryGetValue(word, out var ids))
                {
                    return ids.Count;
                }

                return 0;
            }

            public int GetCount()
            {
                return builder.AllDocuments.Count;
            }

            public void IndexTerm(DocumentId id, string term, int position)
            {
                builder.AddTerm(id, term);
            }

            public IReadOnlyCollection<DocumentId> Search(string word)
            {
                if (builder.RangedPostingsLists.TryGetValue(word, out var blockList))
                {
                    return blockList;
                }

                if (builder.UncompressedPostingsLists.TryGetValue(word, out var ids))
                {
                    return ids;
                }

                return Array.Empty<DocumentId>();
            }
        }
    }
}
