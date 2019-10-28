using System.Collections.Generic;
using System.Linq;

using Moq;
using Xunit;

using Corpus;
using InformationRetrieval.Tokenizers;


namespace InformationRetrieval.Test
{
    using TokenizedBlock = Block<IEnumerable<string>>;

    public class DocumentTokenizerTests
    {
        [Fact]
        public void TokenizeDocumentsTest()
        {
            var docs = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0), "Title 1"),
                    "Title 1. This is, the first document"
                ),
                new Document<string> (
                    new DocumentMetadata(new DocumentId(1), "Title 2"),
                    "Title 2; This is the second. document"
                ),
            };

            var corpus = new List<Block<string>>
            {
                Block<string>.Make(0, docs),
            };

            var tokenized = new List<IEnumerable<TokenizedBlock>>();

            var reader = new Mock<ICorpusReader<string>>();
            reader.Setup(r => r.Read()).Returns(corpus);

            var writer = new Mock<ICorpusWriter<IEnumerable<string>>>();
            writer.Setup(w => w.Write(It.IsAny<IEnumerable<TokenizedBlock>>()))
                  .Callback((IEnumerable<TokenizedBlock> d) => tokenized.Add(d));

            var tokenizer = new DocumentTokenizer(new WordRegexTokenizer(lowerCase: false));
            tokenizer.Transform(reader.Object, writer.Object);

            Assert.Single(tokenized);
            var tokenizedBlock = tokenized[0].First();
            Assert.Equal(2, tokenizedBlock.Documents.Count);
            Assert.Equal("Title 1 This is the first document".Split(), tokenizedBlock.Documents[0].Data);
            Assert.Equal("Title 2 This is the second document".Split(), tokenizedBlock.Documents[1].Data);
        }
    }
}
