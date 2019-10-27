using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;

namespace Corpus
{
    /// <summary>
    /// Implementation of Corpus Reader
    /// </summary>
    public class CorpusZipReader<T> : CorpusZipBase, ICorpusReader<T>
    {
        private readonly IDocumentDataSerializer<T> dataSerializer;
        private readonly CorpusZipMetadata metadata;

        public CorpusZipReader(string path, IDocumentDataSerializer<T> dataSerializer) : this (path, dataSerializer, new FileSystem())
        {
        }

        public CorpusZipReader(string path, IDocumentDataSerializer<T> dataSerializer, IFileSystem fileSystem) : base(path, fileSystem)
        {
            this.dataSerializer = dataSerializer;
            metadata = ReadCorpusZipMetadata();
        }

        public IEnumerable<Block<T>> Read()
        {
            return GetBlocksPaths().Select(ReadBlock);
        }

        public Document<T> ReadDocument(DocumentId docId, bool skipMetadata)
        {
            ushort blockId = (ushort)metadata.GetBlockId(docId);
            var path = GetBlockPath(blockId);
            using var archive = new ZipArchive(FileSystem.File.OpenRead(path), ZipArchiveMode.Read);
            var entry = archive.GetEntry(docId.ToString() + dataSerializer.FileExtension);

            using var stream = entry.Open();
            var data = dataSerializer.Deserialize(stream);

            if (skipMetadata)
            {
                return new Document<T>(new DocumentMetadata(docId, string.Empty), data);
            }
            else
            {
                var metadata = ReadMetadata(archive);
                return new Document<T>(metadata[docId], data);
            }
        }

        public CorpusMetadata ReadMetadata()
        {
            var metadata = GetBlocksPaths().Select(ReadBlockMetadata);
            return new CorpusMetadata(metadata.ToArray());
        }

        private CorpusZipMetadata ReadCorpusZipMetadata()
        {
            using var stream = FileSystem.File.OpenRead(GetCorpusMetadataPath());
            return CorpusZipMetadata.Deserialize(stream);
        }

        private BlockMetadata ReadBlockMetadata(string path)
        {
            using var stream = FileSystem.File.OpenRead(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            return ReadMetadata(archive);
        }

        private Block<T> ReadBlock(string path)
        {
            using var stream = FileSystem.File.OpenRead(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            return ReadArchive(archive);
        }

        private Block<T> ReadArchive(ZipArchive archive)
        {
            var metadata = ReadMetadata(archive);

            return new Block<T>(ReadDocuments(archive, metadata).ToList(), metadata);
        }

        private BlockMetadata ReadMetadata(ZipArchive archive)
        {
            var entry = archive.GetEntry(METADATA_ENTRY_NAME);
            using var stream = entry.Open();

            return BlockMetadata.Deserialize(stream);
        }

        private IEnumerable<Document<T>> ReadDocuments(ZipArchive archive, BlockMetadata metadata)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.Name != METADATA_ENTRY_NAME)
                {
                    using var stream = entry.Open();

                    var data = dataSerializer.Deserialize(stream);
                    var filename = Path.GetFileNameWithoutExtension(entry.Name.AsSpan());
                    var id = DocumentId.Parse(filename);

                    yield return new Document<T>(metadata[id], data);
                }
            }
        }
    }
}
