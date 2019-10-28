using System.Collections.Generic;

using Moq;
using Xunit;

using Corpus;
using InformationRetrieval.Indexing;

namespace InformationRetrieval.Test.Indexing
{
    public class IndexBuilderUnitTests
    {
        [Fact]
        public void IndexBuilderTest()
        {
            var docs1 = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0), "Title 1"),
                    "abc"
                ),
                new Document<string> (
                    new DocumentMetadata(new DocumentId(1), "Title 2"),
                    "defa"
                ),
            };

            var docs2 = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(10), "Title 3"),
                    "bd"
                ),
            };

            var corpus = new List<Block<string>>
            {
                Block<string>.Make(0, docs1),
                Block<string>.Make(1, docs2),
            };

            var indexedWords = new List<(DocumentId, char, int)>();

            var index = new Mock<IBuildableIndex<char>>();
            index.Setup(x => x.IndexTerm(It.IsAny<DocumentId>(), It.IsAny<char>(), It.IsAny<int>()))
                 .Callback<DocumentId, char, int>((id, term, pos) => indexedWords.Add((id, term, pos)));

            var indexer = new IndexBuilder<char, string>(index.Object);
            indexer.IndexCorpus(corpus);

            var expectedIndexedWords = new List<(DocumentId, char, int)>
            {
                (new DocumentId(0), 'a', 0), (new DocumentId(0), 'b', 1), (new DocumentId(0), 'c', 2),
                (new DocumentId(1), 'd', 0), (new DocumentId(1), 'e', 1), (new DocumentId(1), 'f', 2), (new DocumentId(1), 'a', 3),
                (new DocumentId(10), 'b', 0), (new DocumentId(10), 'd', 1),
            };

            Assert.Equal(expectedIndexedWords, indexedWords);
        }
    }
}
