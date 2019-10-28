using System;
using System.IO;
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
        public void BuildBlockedExternalIndexTest_SortBased()
        {
            BuildBlockedExternalIndexTest(SortBasedExternalBuildableIndex<string>.CreateMethod);
        }

        [Fact]
        public void BuildBlockedExternalIndexTest_DictionaryBased_MixedPostingsList()
        {
            BuildBlockedExternalIndexTest(DictonaryBasedExternalBuildableIndex<string>.GetCreateMethodWithMixedPostingsLists(3));
        }

        [Fact]
        public void BuildBlockedExternalIndexTest_DictionaryBased_VarintPostingsList()
        {
            BuildBlockedExternalIndexTest(DictonaryBasedExternalBuildableIndex<string>.GetCreateMethodWithVarintPostingsLists());
        }

        private void BuildBlockedExternalIndexTest(Func<Stream, IExternalBuildableIndex<string>> createIndex)
        {
            var docs = new (DocumentId Id, string[] Text)[]
            {
                (new DocumentId(0), "a b c d".Split()),
                (new DocumentId(1), "d e f a".Split()),
                (new DocumentId(2), "e e f d".Split()),

                (new DocumentId(10), "a b".Split()),
                (new DocumentId(11), "e e h".Split()),

                (new DocumentId(20), "d e f".Split()),
                (new DocumentId(21), "f d g h".Split()),
            };

            var fileSystem = new MockFileSystem();
            fileSystem.Directory.CreateDirectory(path);

            var buildableIndex = new BlockedExternalBuildableIndex<string>(createIndex, path, fileSystem);

            foreach (var doc in docs)
            {
                foreach (var term in doc.Text)
                {
                    buildableIndex.IndexTerm(doc.Id, term, 0);
                }
            }

            var searchableIndex = buildableIndex.Build();

            var allDocs = DocIds(0, 1, 2, 10, 11, 20, 21);
            var aDocs = DocIds(0, 1, 10);
            var dDocs = DocIds(0, 1, 2, 20, 21);
            var eDocs = DocIds(1, 2, 11, 20);
            var gDocs = DocIds(21);
            var hDocs = DocIds(11, 21);

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

        private DocumentId[] DocIds(params uint[] ids)
        {
            return ids.Select(i => new DocumentId(i)).ToArray();
        }
    }
}
