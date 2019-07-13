using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

using InformationRetrieval.Indexing.External;
using Corpus;

namespace InformationRetrieval.Test.Indexing.External
{
    public class BlockedExternalBuildableIndexTest
    {
        private const string path = @"C:\path";

        [Fact]
        public void BuildBlockedExternalIndexTest()
        {
            var docs = new (DocumentId Id, string[] Text)[]
            {
                (new DocumentId(0, 1), "d e f a".Split()),
                (new DocumentId(0, 0), "a b c d".Split()),
                (new DocumentId(0, 2), "e e f d".Split()),

                (new DocumentId(1, 0), "a b".Split()),
                (new DocumentId(1, 1), "e e h".Split()),

                (new DocumentId(2, 0), "d e f".Split()),
                (new DocumentId(2, 1), "f d g h".Split()),
            };

            var fileSystem = new MockFileSystem();
            fileSystem.Directory.CreateDirectory(path);

            var buildableIndex = new BlockedExternalBuildableIndex<string>(
                SortBasedExternalBuildableIndex<string>.CreateMethod, 
                path, fileSystem);

            foreach (var doc in docs)
            {
                foreach (var term in doc.Text)
                {
                    buildableIndex.IndexTerm(doc.Id, term, 0);
                }
            }

            var searchableIndex = buildableIndex.Build();

            var allDocs = DocIds((0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (2, 0), (2, 1));
            var aDocs = DocIds((0, 0), (0, 1), (1, 0));
            var dDocs = DocIds((0, 0), (0, 1), (0, 2), (2, 0), (2, 1));
            var eDocs = DocIds((0, 1), (0, 2), (1, 1), (2, 0));
            var gDocs = DocIds((2, 1));
            var hDocs = DocIds((1, 1), (2, 1));

            Assert.Equal(allDocs, searchableIndex.GetAll().ToArray());
            Assert.Equal(aDocs, searchableIndex.Search("a").ToArray());
            Assert.Equal(dDocs, searchableIndex.Search("d").ToArray());
            Assert.Equal(eDocs, searchableIndex.Search("e").ToArray());
            Assert.Equal(gDocs, searchableIndex.Search("g").ToArray());
            Assert.Equal(hDocs, searchableIndex.Search("h").ToArray());

            Assert.Equal(allDocs.Length, searchableIndex.GetCount());
            Assert.Equal(aDocs.Length, searchableIndex.GetCount("a"));
            Assert.Equal(dDocs.Length, searchableIndex.GetCount("d"));
            Assert.Equal(eDocs.Length, searchableIndex.GetCount("e"));
            Assert.Equal(gDocs.Length, searchableIndex.GetCount("g"));
            Assert.Equal(hDocs.Length, searchableIndex.GetCount("h"));
        }

        private DocumentId[] DocIds(params (ushort blockId, ushort localId)[] ids)
        {
            return ids.Select(i => new DocumentId(i.blockId, i.localId)).ToArray();
        }
    }
}
