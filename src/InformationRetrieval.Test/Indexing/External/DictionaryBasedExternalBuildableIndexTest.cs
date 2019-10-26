using System.IO;
using System.Linq;

using Xunit;

using Corpus;
using InformationRetrieval.Indexing.External;
using InformationRetrieval.Indexing.PostingsList;

namespace InformationRetrieval.Test.Indexing.External
{
    public class DictionaryBasedExternalBuildableIndexTests : IndexUnitTestsBase<ExternalIndex<string>>
    {
        private const int RangeThreshold = 2;

        [Fact]
        public void ExternalIndexBuildTest()
        {
            var stream = new MemoryStream();
            var postingsListBuilder = new MixedPostingsListBuilder<string>(RangeThreshold);
            var buildableIndex = new DictonaryBasedExternalBuildableIndex<string>(postingsListBuilder, stream);

            var docs = new (DocumentId Id, string[] Text)[]
            {
                (new DocumentId(0), "a b c d".Split()),
                (new DocumentId(1), "e e f d".Split()),
                (new DocumentId(2), "d e f a".Split()),
            };

            foreach (var doc in docs)
            {
                foreach (var term in doc.Text)
                {
                    buildableIndex.IndexTerm(doc.Id, term, 0);
                }
            }

            var searchableIndex = buildableIndex.Build();

            var allDocs = new[] { new DocumentId(0), new DocumentId(1), new DocumentId(2) };
            var aDocs = new[] { new DocumentId(0), new DocumentId(2) };
            var dDocs = new[] { new DocumentId(0), new DocumentId(1), new DocumentId(2) };
            var eDocs = new[] { new DocumentId(1), new DocumentId(2) };

            Assert.Equal(allDocs, searchableIndex.GetAll().ToArray());
            Assert.Equal(aDocs, searchableIndex.Search("a").ToArray());
            Assert.Equal(dDocs, searchableIndex.Search("d").ToArray());
            Assert.Equal(eDocs, searchableIndex.Search("e").ToArray());

            Assert.Equal(allDocs.Length, searchableIndex.GetCount());
            Assert.Equal(aDocs.Length, searchableIndex.GetCount("a"));
            Assert.Equal(dDocs.Length, searchableIndex.GetCount("d"));
            Assert.Equal(eDocs.Length, searchableIndex.GetCount("e"));
        }

        protected override ExternalIndex<string> CreateIndex(string[][] corpus)
        {
            var stream = new MemoryStream();
            var postingsListBuilder = new MixedPostingsListBuilder<string>(RangeThreshold);
            var buildableIndex = new DictonaryBasedExternalBuildableIndex<string>(postingsListBuilder, stream);
            IndexHelper.BuildIndex(buildableIndex, corpus);
            return buildableIndex.BuildExternalIndex();
        }
    }
}
