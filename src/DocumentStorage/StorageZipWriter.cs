using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;

namespace DocumentStorage
{
    /// <summary>
    /// Implementation of Storage Writer
    /// </summary>
    public class StorageZipWriter<T> : StorageZipBase, IStorageWriter<T>
    {
        private readonly IDocumentDataSerializer<T> dataSerializer;

        public StorageZipWriter(string path, IDocumentDataSerializer<T> dataSerializer) : this(path, dataSerializer, new FileSystem())
        {
        }

        public StorageZipWriter(string path, IDocumentDataSerializer<T> dataSerializer, IFileSystem fileSystem) : base(path, fileSystem)
        {
            this.dataSerializer = dataSerializer;
        }

        public void Write(IEnumerable<DocumentCollection<T>> storage)
        {
            foreach (var collection in storage)
            {
                var collectionPath = GetCollectionPath(collection.Metadata.Id);
                SaveCollection(collection, collectionPath);
            }
        }

        private void SaveCollection(DocumentCollection<T> collection, string path)
        {
            using var stream = FileSystem.File.OpenWrite(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            WriteMetadata(archive, collection.Metadata);
            SaveDocuments(archive, collection.Documents);
        }

        private void WriteMetadata(ZipArchive archive, DocumentCollectionMetadata metadata)
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

    }
}
