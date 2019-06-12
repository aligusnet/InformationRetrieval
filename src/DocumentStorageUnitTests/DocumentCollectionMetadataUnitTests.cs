using Xunit;

using DocumentStorage;
using System.IO;

namespace DocumentStorageUnitTests
{
    public class DocumentCollectionMetadataUnitTests
    {
        [Fact]
        public void MetadataSerializationTest()
        {
            var props = new []
            {
                new DocumentMetadata(new DocumentId(100, 0), "Title 1"),
                new DocumentMetadata(new DocumentId(100, 1), "Title 2"),
                new DocumentMetadata(new DocumentId(100, 2), "Title 3"),
            };
            var metadata = new DocumentCollectionMetadata(100, props);

            var stream = new MemoryStream();

            metadata.Serialize(stream);

            stream.Seek(0, SeekOrigin.Begin);

            var deserializedMetadata = DocumentCollectionMetadata.Deserialize(stream);

            Assert.Equal(metadata.Id, deserializedMetadata.Id);

            foreach (var kv in props)
            {
                Assert.Equal(kv.Id.Id, deserializedMetadata[kv.Id].Id.Id);
                Assert.Equal(kv.Title, deserializedMetadata[kv.Id].Title);
            }
        }
    }
}
