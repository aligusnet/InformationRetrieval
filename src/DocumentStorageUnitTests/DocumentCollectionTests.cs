using System;
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
                new Document<string>
                {
                    Id = Guid.NewGuid(),
                    Title = "Title 1",
                    Data = "Title 1. This is the first document",
                },
                new Document<string>
                {
                    Id = Guid.NewGuid(),
                    Title = "Title 2",
                    Data = "Title 2. This is the second document",
                },
                new Document<string>
                {
                    Id = Guid.NewGuid(),
                    Title = "Title 3",
                    Data = "Title 3. This is thethirs document",
                },
            };

            var collection = DocumentCollection<string>.Make(docs);

            Assert.Equal(docs.Count, collection.Documents.Count);
            Assert.Equal(docs.Count, collection.Metadata.Count);
            Assert.Equal(docs[1].Title, collection.Metadata[docs[1].Id].Title);
        }
    }
}
