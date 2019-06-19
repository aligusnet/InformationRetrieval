using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using NaturalLanguageTools.Indexing;
using DocumentStorage;

namespace NaturalLanguageToolsUnitTests.Indexing
{
    public class BooleanSearchEngineUnitTests
    {
        [Fact]
        public void TermSearchTest()
        {
            var index = IndexHelper.CreateDictionaryIndex();
            var engine = new BooleanSearchEngine<string>(index);

            foreach (var kv in IndexHelper.Results)
            {
                var query = BooleanQuery<string>.CreateTerm(kv.Key);
                Assert.Equal(kv.Value, engine.ExecuteQuery(query).ToArray());
            }
        }

        [Fact]
        public void OperatorAndSearchTest()
        {
            var index = IndexHelper.CreateDictionaryIndex();
            var engine = new BooleanSearchEngine<string>(index);

            var tests = new List<(BooleanQuery<string> Query, DocumentId[] Result)>
            {
                (BooleanQuery<string>.CreateAnd("great", "largest"), new [] { new DocumentId(0, 0) }),
                (BooleanQuery<string>.CreateAnd("pacific", "ocean", "earth"), new [] { new DocumentId(0, 2), new DocumentId(1, 2) }),
                (BooleanQuery<string>.CreateAnd("earth", "largest", "ocean"), new [] { new DocumentId(1, 2) }),
                (BooleanQuery<string>.CreateAnd("earth", "largest", "ocean", "pacific"), new [] { new DocumentId(1, 2) }),
                (BooleanQuery<string>.CreateAnd("earth", "largest", "ocean", "atlantic"), Array.Empty<DocumentId>()),
            };

            foreach (var test in tests)
            {
                Assert.Equal(test.Result, engine.ExecuteQuery(test.Query).ToArray());
            }
        }

        [Fact]
        public void OperatorOrSearchTest()
        {
            var index = IndexHelper.CreateDictionaryIndex();
            var engine = new BooleanSearchEngine<string>(index);

            var tests = new List<(BooleanQuery<string> Query, DocumentId[] Result)>
            {
                (BooleanQuery<string>.CreateOr("great", "largest"), new [] { new DocumentId(0, 0), new DocumentId(1, 2), new DocumentId(1, 3) }),
                (BooleanQuery<string>.CreateOr("pacific", "ocean", "earth"), new [] { new DocumentId(0, 2), new DocumentId(1, 2), new DocumentId(1, 3) }),
                (BooleanQuery<string>.CreateOr("earth", "great", "ocean"), new [] { new DocumentId(0, 0), new DocumentId(0, 2), new DocumentId(1, 2), new DocumentId(1, 3),  }),
            };

            foreach (var test in tests)
            {
                Assert.Equal(test.Result, engine.ExecuteQuery(test.Query).ToArray());
            }
        }
    }
}
