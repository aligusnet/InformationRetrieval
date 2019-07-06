using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace Corpus.Test
{
    public class CorpusZipTests
    {
        private const string path = @"C:\path";

        [Fact]
        public void ZipReadWriteTest()
        {
            var corpus = GenerateCorpusData();
            var fileSystem = SerializeCorpus(corpus);
            var reader = new CorpusZipReader<string>(path, new StringDocumentDataSerializer(), fileSystem);

            var deserializedCorpus = reader.Read().ToList();

            Assert.Equal(corpus.Count, deserializedCorpus.Count);
            Assert.Equal(corpus[0].Documents[1].Metadata.Id, deserializedCorpus[0].Documents[1].Metadata.Id);
            Assert.Equal(corpus[0].Documents[1].Data, deserializedCorpus[0].Documents[1].Data);
        }

        [Theory]
        [InlineData(1, 0, true)]
        [InlineData(0, 1, false)]
        public void ReadDocumentTest(ushort blockId, ushort localId, bool skipMetadata)
        {
            var corpus = GenerateCorpusData();
            var fileSystem = SerializeCorpus(corpus);
            var reader = new CorpusZipReader<string>(path, new StringDocumentDataSerializer(), fileSystem);

            var docId = new DocumentId(blockId, localId);
            var actual = reader.ReadDocument(docId, skipMetadata);
            var expected = corpus[blockId].Documents[localId];

            Assert.Equal(expected.Metadata.Id, actual.Metadata.Id);
            Assert.Equal(expected.Data, actual.Data);
            if (!skipMetadata)
            {
                Assert.Equal(expected.Metadata.Title, actual.Metadata.Title);
            }
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        public void ReadMetadataTest(ushort blockId, ushort localId)
        {
            var corpus = GenerateCorpusData();
            var fileSystem = SerializeCorpus(corpus);
            var reader = new CorpusZipReader<string>(path, new StringDocumentDataSerializer(), fileSystem);

            var docId = new DocumentId(blockId, localId);
            var metadata = reader.ReadMetadata();
            var expected = corpus[blockId].Documents[localId].Metadata;
            var actual = metadata[docId];

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Title, actual.Title);
        }

        private static IList<Block<string>> GenerateCorpusData()
        {
            var docs1 = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 0), "Title 1"),
                    "Title 1. This is the first document"
                ),
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 1), "Title 2"),
                    "Title 2. This is the second document"
                ),
                new Document<string> (
                    new DocumentMetadata(new DocumentId(0, 2), "Title 3"),
                    "Title 3. This is thethirs document"
                ),
            };

            var docs2 = new List<Document<string>>
            {
                new Document<string> (
                    new DocumentMetadata(new DocumentId(1, 0), "Title 4"),
                    "Title 4. This is the first document from the second block"
                ),
            };

            return new List<Block<string>>
            {
                Block<string>.Make(0, docs1),
                Block<string>.Make(1, docs2),
            };
        }

        private static MockFileSystem SerializeCorpus(IList<Block<string>> corpus)
        {
            var fileSystem = new MockFileSystem();
            fileSystem.Directory.CreateDirectory(path);
            var writer = new CorpusZipWriter<string>(path, new StringDocumentDataSerializer(), fileSystem);
            writer.Write(corpus);
            return fileSystem;
        }
    }
}
