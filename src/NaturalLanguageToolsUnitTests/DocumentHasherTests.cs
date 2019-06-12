using System.Collections.Generic;
using System.Linq;

using Moq;
using Xunit;

using DocumentStorage;
using NaturalLanguageTools;

namespace NaturalLanguageToolsUnitTests
{
    using Tokens = IEnumerable<string>;
    using HashedCollection = DocumentCollection<int[]>;

    public class DocumentHasherTests
    {
        
        [Fact]
        public void HashDocumentTest()
        {
            var storage = CreateStorage();

            var hasher = new DocumentHasher();

            var hashed = hasher.Transform(storage).First().Documents;

            Assert.Equal(7, hashed[0].Data.Length);
            Assert.Equal(7, hashed[1].Data.Length);
            Assert.NotEqual(hashed[0].Data, hashed[1].Data);

            hashed[0].Data[1] = hashed[1].Data[1];
            hashed[0].Data[5] = hashed[1].Data[5];
            Assert.Equal(hashed[0].Data, hashed[1].Data);
        }

        [Fact]
        public void HasherIsConsistentTest()
        {
            var storage = CreateStorage();

            var hashed1 = new DocumentHasher().Transform(storage).First().Documents;
            var hashed2 = new DocumentHasher().Transform(storage).First().Documents;

            Assert.Equal(hashed1[0].Data, hashed2[0].Data);
            Assert.Equal(hashed1[1].Data, hashed2[1].Data);
        }

        [Fact]
        public void HashDocumentStorageTest()
        {
            var storage = CreateStorage();

            var hashed = new List<IEnumerable<DocumentCollection<int[]>>>();

            var reader = new Mock<IStorageReader<Tokens>>();
            reader.Setup(r => r.Read()).Returns(storage);

            var writer = new Mock<IStorageWriter<int[]>>();
            writer.Setup(w => w.Write(It.IsAny<IEnumerable<HashedCollection>>()))
                  .Callback((IEnumerable<HashedCollection> d) => hashed.Add(d));

            var tokenizer = new DocumentHasher();
            tokenizer.Transform(reader.Object, writer.Object);

            Assert.Single(hashed);
            var hashedCollection = hashed[0].First();
            Assert.Equal(2, hashedCollection.Documents.Count);
        }

        private static IEnumerable<DocumentCollection<Tokens>> CreateStorage()
        {
            var docs = new List<Document<Tokens>>
            {
                new Document<Tokens> (
                    new DocumentMetadata(new DocumentId(0, 0), "Title 1"),
                    "Title 1 This is the first document".Split()
                ),
                new Document<Tokens> (
                    new DocumentMetadata(new DocumentId(0, 1), "Title 2"),
                    "Title 2 This is the second document".Split()
                ),
            };

            return new List<DocumentCollection<Tokens>>
            {
                DocumentCollection<Tokens>.Make(0, docs),
            };
        }
    }
}
