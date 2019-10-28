using Xunit;

using System.IO;

namespace Corpus.Test
{
    public class BlockMetadataUnitTests
    {
        [Fact]
        public void MetadataSerializationTest()
        {
            var props = new []
            {
                new DocumentMetadata(new DocumentId(1000), "Title 1"),
                new DocumentMetadata(new DocumentId(1001), "Title 2"),
                new DocumentMetadata(new DocumentId(1002), "Title 3"),
            };
            var metadata = new BlockMetadata(100, props);

            var stream = new MemoryStream();

            metadata.Serialize(stream);

            stream.Seek(0, SeekOrigin.Begin);

            var deserializedMetadata = BlockMetadata.Deserialize(stream);

            Assert.Equal(metadata.Id, deserializedMetadata.Id);

            foreach (var kv in props)
            {
                Assert.Equal(kv.Id.Id, deserializedMetadata[kv.Id].Id.Id);
                Assert.Equal(kv.Title, deserializedMetadata[kv.Id].Title);
            }
        }
    }
}
