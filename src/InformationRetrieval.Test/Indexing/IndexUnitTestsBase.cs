using System.Linq;

using Xunit;

using InformationRetrieval.Indexing;

namespace InformationRetrieval.Test.Indexing
{
    public abstract class IndexUnitTestsBase<TIndex> where TIndex : ISearchableIndex<string>
    {
        [Fact]
        public void IndexSearchTest()
        {
            var index = CreateIndex();

            foreach (var kv in IndexHelper.Results)
            {
                Assert.Equal(kv.Value, index.Search(kv.Key).ToArray());
                Assert.Equal(kv.Value.Length, index.GetCount(kv.Key));
            }

            Assert.Equal(IndexHelper.AllDocuments, index.GetAll());
            Assert.Equal(IndexHelper.AllDocuments.Length, index.GetCount());
        }

        protected void AssertIndices(ISearchableIndex<string> expected, ISearchableIndex<string> actual)
        {
            foreach (var word in IndexHelper.Results.Keys)
            {
                Assert.Equal(expected.Search(word), actual.Search(word));
                Assert.Equal(expected.GetCount(word), actual.GetCount(word));
            }

            Assert.Equal(expected.GetAll(), actual.GetAll());
            Assert.Equal(expected.GetCount(), actual.GetCount());
        }

        protected abstract TIndex CreateIndex(string[][] corpus);

        protected TIndex CreateIndex()
        {
            return CreateIndex(IndexHelper.GetTestSentenceBlocks());
        }
    }
}
