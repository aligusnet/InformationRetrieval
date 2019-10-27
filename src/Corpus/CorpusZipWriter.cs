using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;

namespace Corpus
{
    /// <summary>
    /// Implementation of Corpus Writer
    /// </summary>
    public class CorpusZipWriter<T> : CorpusZipBase, ICorpusWriter<T>
    {
        private readonly IDocumentDataSerializer<T> dataSerializer;

        public CorpusZipWriter(string path, IDocumentDataSerializer<T> dataSerializer) : this(path, dataSerializer, new FileSystem())
        {
        }

        public CorpusZipWriter(string path, IDocumentDataSerializer<T> dataSerializer, IFileSystem fileSystem) : base(path, fileSystem)
        {
            this.dataSerializer = dataSerializer;
        }

        public void Write(IEnumerable<Block<T>> corpus)
        {
            var firstDocumentIdInBlock = new List<DocumentId>();

            foreach (var block in corpus)
            {
                var path = GetBlockPath(block.Metadata.Id);
                SaveBlock(block, path);

                firstDocumentIdInBlock.Add(block.Documents[0].Metadata.Id);
            }

            SaveCorpusMetadata(new CorpusZipMetadata(firstDocumentIdInBlock));
        }

        private void SaveBlock(Block<T> block, string path)
        {
            using var stream = FileSystem.File.OpenWrite(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            WriteMetadata(archive, block.Metadata);
            SaveDocuments(archive, block.Documents);
        }

        private void WriteMetadata(ZipArchive archive, BlockMetadata metadata)
        {
            ZipArchiveEntry contentsEntry = archive.CreateEntry(METADATA_ENTRY_NAME);
            using var stream = contentsEntry.Open();
            metadata.Serialize(stream);
        }

        private void SaveDocuments(ZipArchive archive, IEnumerable<Document<T>> docs)
        {
            foreach (var doc in docs)
            {
                var entry = archive.CreateEntry(doc.Metadata.Id.ToString() + dataSerializer.FileExtension);
                using var stream = entry.Open();
                dataSerializer.Serialize(stream, doc.Data);
            }
        }

        private void SaveCorpusMetadata(CorpusZipMetadata metadata)
        {
            using var stream = FileSystem.File.OpenWrite(GetCorpusMetadataPath());
            metadata.Serialize(stream);
        }
    }
}
