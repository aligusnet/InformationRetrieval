using System;
using System.Collections.Generic;

using Corpus;
using InformationRetrieval.Indexing;
using InformationRetrieval.Indexing.PostingsList;

namespace InformationRetrieval.Test.Indexing.PostingsList
{
    class VarintPostingsListBuilderTest : IndexUnitTestsBase<VarintPostingsListBuilderTest.Index>
    {
        protected override Index CreateIndex(string[][] corpus)
        {
            var index = new Index();
            IndexHelper.BuildIndex(index, corpus);
            return index;
        }

        public class Index : IBuildableIndex<string>, ISearchableIndex<string>
        {
            VarintPostingsListBuilder<string> builder = new VarintPostingsListBuilder<string>();

            public ISearchableIndex<string> Build()
            {
                return this;
            }

            public IReadOnlyCollection<DocumentId> GetAll()
            {
                return builder.Documents;
            }

            public int GetCount(string word)
            {
                if (builder.VarintPostingsLists.TryGetValue(word, out var blockList))
                {
                    return blockList.Count;
                }

                return 0;
            }

            public int GetCount()
            {
                return builder.Documents.Count;
            }

            public void IndexTerm(DocumentId id, string term, int position)
            {
                builder.AddTerm(id, term);
            }

            public IReadOnlyCollection<DocumentId> Search(string word)
            {
                if (builder.VarintPostingsLists.TryGetValue(word, out var ps))
                {
                    return ps;
                }

                return Array.Empty<DocumentId>();
            }
        }
    }
}
