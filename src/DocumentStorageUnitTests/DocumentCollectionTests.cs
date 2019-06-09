using System.Collections.Generic;
using Xunit;

using DocumentStorage;

namespace DocumentStorageUnitTests
{
    public class DocumentCollectionTests
    {
        [Fact]
        public void MakeDocumentCollectionTest()
        {
            var docs = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentId(0, 0),
                    "Title 1",
                    "Title 1. This is the first document"
                ),
                new Document<string> (
                    new DocumentId(0, 1),
                    "Title 2",
                    "Title 2. This is the second document"
                ),
                new Document<string>
                (
                    new DocumentId(0, 2),
                    "Title 3",
                    "Title 3. This is thethirs document"
                ),
            };

            var collection = DocumentCollection<string>.Make(docs);

            Assert.Equal(docs.Count, collection.Documents.Count);
            Assert.Equal(docs.Count, collection.Metadata.Count);
            Assert.Equal(docs[1].Title, collection.Metadata[docs[1].Id].Title);
        }
    }
}
