using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;

namespace DocumentStorage
{
    /// <summary>
    /// Implementation of Storage Reader
    /// </summary>
    public class StorageZipReader<T> : StorageZipBase, IStorageReader<T>
    {
        private readonly IDocumentDataSerializer<T> dataSerializer;

        public StorageZipReader(string path, IDocumentDataSerializer<T> dataSerializer) : this (path, dataSerializer, new FileSystem())
        {
        }

        public StorageZipReader(string path, IDocumentDataSerializer<T> dataSerializer, IFileSystem fileSystem) : base(path, fileSystem)
        {
            this.dataSerializer = dataSerializer;
        }

        public IEnumerable<DocumentCollection<T>> Read()
        {
            return GetCollectionsPaths().Select(ReadDocumentCollection);
        }

        public Document<T> ReadDocument(DocumentId docId, bool skipMetadata)
        {
            var path = GetCollectionPath(docId.CollectionId);
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

        public DocumentStorageMetadata ReadMetadata()
        {
            var metadata = GetCollectionsPaths().Select(ReadDocumentCollectionMetadata);
            return new DocumentStorageMetadata(metadata.ToArray());
        }

        private DocumentCollectionMetadata ReadDocumentCollectionMetadata(string path)
        {
            using var stream = FileSystem.File.OpenRead(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            return ReadMetadata(archive);
        }

        private DocumentCollection<T> ReadDocumentCollection(string path)
        {
            using var stream = FileSystem.File.OpenRead(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            return ReadArchive(archive);
        }

        private DocumentCollection<T> ReadArchive(ZipArchive archive)
        {
            var metadata = ReadMetadata(archive);

            return new DocumentCollection<T>(ReadDocuments(archive, metadata).ToList(), metadata);
        }

        private DocumentCollectionMetadata ReadMetadata(ZipArchive archive)
        {
            var entry = archive.GetEntry(METADATA_ENTRY_NAME);
            using var stream = entry.Open();

            return DocumentCollectionMetadata.Deserialize(stream);
        }

        private IEnumerable<Document<T>> ReadDocuments(ZipArchive archive, DocumentCollectionMetadata metadata)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.Name != METADATA_ENTRY_NAME)
                {
                    using var stream = entry.Open();

                    var data = dataSerializer.Deserialize(stream);
                    var id = DocumentId.Parse((Path.GetFileNameWithoutExtension(entry.Name)));

                    yield return new Document<T>(metadata[id], data);
                }
            }
        }
    }
}
