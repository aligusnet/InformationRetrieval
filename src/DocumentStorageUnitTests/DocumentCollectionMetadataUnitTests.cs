using System.Collections.Generic;
using Xunit;

using DocumentStorage;
using System.Linq;
using System.IO;

namespace DocumentStorageUnitTests
{
    public class DocumentCollectionMetadataUnitTests
    {
        [Fact]
        public void MetadataSerializationTest()
        {
            var props = new List<DocumentMetadata>
            {
                new DocumentMetadata(new DocumentId(100, 1201), "Title 1"),
                new DocumentMetadata(new DocumentId(100, 1202), "Title 2"),
                new DocumentMetadata(new DocumentId(100, 1203), "Title 3"),
            };
            var metadata = new DocumentCollectionMetadata(props.ToDictionary(p => p.Id));

            var stream = new MemoryStream();

            metadata.Serialize(stream);

            stream.Seek(0, SeekOrigin.Begin);

            var deserializedMetadata = DocumentCollectionMetadata.Deserialize(stream);

            foreach (var kv in props)
            {
                Assert.Equal(kv.Id.Id, deserializedMetadata[kv.Id].Id.Id);
                Assert.Equal(kv.Title, deserializedMetadata[kv.Id].Title);
            }
        }
    }
}
