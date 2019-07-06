using System.Linq;

using Xunit;

using NaturalLanguageTools.Indexing;
using Corpus;
using System.IO;

namespace NaturalLanguageTools.Test.Indexing
{
    public class SortBasedExternalBuildableIndexTests : IndexUnitTestsBase<ExternalIndex<string>>
    {
        [Fact]
        public void ExternalIndexBuildTest()
        {
            var stream = new MemoryStream();
            var buildableIndex = new SortBasedExternalBuildableIndex<string>(stream);

            var docs = new (DocumentId Id, string[] Text)[]
            {
                (new DocumentId(2), "d e f a".Split()),
                (new DocumentId(0), "a b c d".Split()),
                (new DocumentId(1), "e e f d".Split()),
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
            var buildableIndex = new SortBasedExternalBuildableIndex<string>(stream);
            IndexHelper.BuildIndex(buildableIndex, corpus);
            return buildableIndex.Build();
        }
    }
}
