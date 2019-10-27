using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Corpus.Test
{
    public class CorpusZipMetadataTest
    {
        [Fact]
        public void MetadataSerializationTest()
        {
            var metadata = new CorpusZipMetadata(new List<uint> { 0, 111, 34567, 345346 }.Select(u => new DocumentId(u)).ToList());

            var stream = new MemoryStream();

            metadata.Serialize(stream);

            stream.Seek(0, SeekOrigin.Begin);

            var deserializedMetadata = CorpusZipMetadata.Deserialize(stream);

            Assert.Equal(metadata.FirstDocumentIdInBlock, deserializedMetadata.FirstDocumentIdInBlock);
        }

        [Fact]
        public void GetBlockIdTest()
        {
            var metadata = new CorpusZipMetadata(new List<uint> { 0, 111, 34567, 345346 }.Select(u => new DocumentId(u)).ToList());

            Assert.Equal(0, metadata.GetBlockId(new DocumentId(0)));
            Assert.Equal(0, metadata.GetBlockId(new DocumentId(100)));
            Assert.Equal(0, metadata.GetBlockId(new DocumentId(110)));
            Assert.Equal(1, metadata.GetBlockId(new DocumentId(111)));
            Assert.Equal(1, metadata.GetBlockId(new DocumentId(200)));
            Assert.Equal(3, metadata.GetBlockId(new DocumentId(2_000_000)));
        }
    }
}
