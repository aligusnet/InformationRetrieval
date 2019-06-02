using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using Xunit;

using DocumentStorage;
using NaturalLanguageTools;


namespace NaturalLanguageToolsUnitTests
{
    using TokenizedCollection = DocumentCollection<IEnumerable<string>>;

    public class DocumentTokenizerTests
    {
        [Fact]
        public void TokenizeDocumentsTest()
        {
            var docs = new List<Document<string>>
            {
                new Document<string> (
                    Guid.NewGuid(),
                    "Title 1",
                    "Title 1. This is, the first document"
                ),
                new Document<string> (
                    Guid.NewGuid(),
                    "Title 2",
                    "Title 2; This is the second. document"
                ),
            };

            var storage = new List<DocumentCollection<string>>
            {
                DocumentCollection<string>.Make(docs),
            };

            var tokenized = new List<IEnumerable<TokenizedCollection>>();

            var reader = new Mock<IStorageReader<string>>();
            reader.Setup(r => r.Read()).Returns(storage);

            var writer = new Mock<IStorageWriter<IEnumerable<string>>>();
            writer.Setup(w => w.Write(It.IsAny<IEnumerable<TokenizedCollection>>()))
                  .Callback((IEnumerable<TokenizedCollection> d) => tokenized.Add(d));

            var tokenizer = new DocumentTokenizer(new WordRegexTokenizer());
            tokenizer.Transform(reader.Object, writer.Object);

            Assert.Single(tokenized);
            var tokenizedCollection = tokenized[0].First();
            Assert.Equal(2, tokenizedCollection.Documents.Count);
            Assert.Equal("Title 1 This is the first document".Split(), tokenizedCollection.Documents[0].Data);
            Assert.Equal("Title 2 This is the second document".Split(), tokenizedCollection.Documents[1].Data);
        }
    }
}
