using System.Collections.Generic;
using Xunit;

namespace Corpus.Test
{
    public class BlockTests
    {
        [Fact]
        public void MakeBlockTest()
        {
            var docs = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 0), "Title 1"),
                    "Title 1. This is the first document"
                ),
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 1), "Title 2"),
                    "Title 2. This is the second document"
                ),
                new Document<string>
                (
                    new DocumentMetadata(new DocumentId(0, 2), "Title 3"),
                    "Title 3. This is thethirs document"
                ),
            };

            var block = Block<string>.Make(0, docs);

            Assert.Equal(docs.Count, block.Documents.Count);
            Assert.Equal(docs.Count, block.Metadata.Count);
            Assert.Equal(docs[1].Metadata.Title, block.Metadata[docs[1].Metadata.Id].Title);
        }
    }
}
