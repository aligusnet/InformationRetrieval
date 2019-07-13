using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using InformationRetrieval.BooleanSearch;
using InformationRetrieval.Indexing.InMemory;
using Corpus;

using InformationRetrieval.Test.Indexing;

namespace InformationRetrieval.Test.BooleanSearch
{
    public class BooleanSearchEngineUnitTests
    {
        [Fact]
        public void TermSearchTest()
        {
            var index = CreateDictionaryIndex();
            var engine = new BooleanSearchEngine<string>(index, s => s.ToLower());

            foreach (var kv in IndexHelper.Results)
            {
                var query = BooleanQuery.CreateTerm(kv.Key);
                Assert.Equal(kv.Value, engine.ExecuteQuery(query).ToArray());
            }
        }

        [Fact]
        public void OperatorAndSearchTest()
        {
            var index = CreateDictionaryIndex();
            var engine = new BooleanSearchEngine<string>(index, s => s.ToLower());

            var tests = new List<(BooleanQuery Query, DocumentId[] Result)>
            {
                (BooleanQuery.CreateAnd("great", "largest"), new [] { new DocumentId(0, 0) }),
                (BooleanQuery.CreateAnd("pacific", "ocean", "earth"), new [] { new DocumentId(0, 2), new DocumentId(1, 2) }),
                (BooleanQuery.CreateAnd("earth", "largest", "ocean"), new [] { new DocumentId(1, 2) }),
                (BooleanQuery.CreateAnd("earth", "largest", "ocean", "pacific"), new [] { new DocumentId(1, 2) }),
                (BooleanQuery.CreateAnd("earth", "largest", "ocean", "atlantic"), Array.Empty<DocumentId>()),
                (BooleanQuery.CreateAnd("moon"), Array.Empty<DocumentId>()),
                (BooleanQuery.CreateAnd("the"), IndexHelper.AllDocuments),
            };

            foreach (var test in tests)
            {
                Assert.Equal(test.Result, engine.ExecuteQuery(test.Query).ToArray());
            }
        }

        [Fact]
        public void OperatorOrSearchTest()
        {
            var index = CreateDictionaryIndex();
            var engine = new BooleanSearchEngine<string>(index, s => s.ToLower());

            var tests = new List<(BooleanQuery Query, DocumentId[] Result)>
            {
                (BooleanQuery.CreateOr("great", "largest"), new [] { new DocumentId(0, 0), new DocumentId(1, 2), new DocumentId(1, 3) }),
                (BooleanQuery.CreateOr("pacific", "ocean", "earth"), new [] { new DocumentId(0, 2), new DocumentId(1, 2), new DocumentId(1, 3) }),
                (BooleanQuery.CreateOr("earth", "great", "ocean"), new [] { new DocumentId(0, 0), new DocumentId(0, 2), new DocumentId(1, 2), new DocumentId(1, 3), }),
                (BooleanQuery.CreateOr("moon"), Array.Empty<DocumentId>()),
                (BooleanQuery.CreateOr("the", "a"), IndexHelper.AllDocuments),
            };

            foreach (var test in tests)
            {
                Assert.Equal(test.Result, engine.ExecuteQuery(test.Query).ToArray());
            }
        }

        [Fact]
        public void OperatorNotSearchTest()
        {
            var index = CreateDictionaryIndex();
            var engine = new BooleanSearchEngine<string>(index, s => s.ToLower());

            var tests = new List<(BooleanQuery Query, DocumentId[] Result)>
            {
                (BooleanQuery.CreateOr("earth", "great", "ocean"), new [] { new DocumentId(0, 1), new DocumentId(1, 0), new DocumentId(1, 1), }),
                (BooleanQuery.CreateOr("pacific", "ocean", "earth"), new [] { new DocumentId(0, 0), new DocumentId(0, 1), new DocumentId(1, 0), new DocumentId(1, 1), }),
                (BooleanQuery.CreateTerm("moon"), IndexHelper.AllDocuments),
                (BooleanQuery.CreateTerm("the"), Array.Empty<DocumentId>()),
            };

            foreach (var test in tests)
            {
                Assert.Equal(test.Result, engine.ExecuteQuery(BooleanQuery.CreateNot(test.Query)).ToArray());
            }
        }

        private static DictionaryIndex<string> CreateDictionaryIndex()
            => CreateDictionaryIndex(IndexHelper.GetTestSentenceBlocks());

        public static DictionaryIndex<string> CreateDictionaryIndex(string[][] corpus)
        {
            var index = new DictionaryIndex<string>(rareWordThreshold: 3);
            IndexHelper.BuildIndex(index, corpus);
            return index;
        }
    }
}
