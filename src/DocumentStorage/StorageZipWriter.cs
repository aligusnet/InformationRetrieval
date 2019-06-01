using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using Newtonsoft.Json;

namespace DocumentStorage
{
    /// <summary>
    /// Implementation of Storage Writer
    /// </summary>
    public class StorageZipWriter<T> : IStorageWriter<T>
    {
        internal const string METADATA_ENTRY_NAME = "_METADATA_.json";

        private readonly string path;
        private readonly IDocumentDataSerializer<T> dataSerializer;
        private readonly IFileSystem fileSystem;

        public StorageZipWriter(string path, IDocumentDataSerializer<T> dataSerializer) : this(path, dataSerializer, new FileSystem())
        {
        }

        public StorageZipWriter(string path, IDocumentDataSerializer<T> dataSerializer, IFileSystem fileSystem)
        {
            this.path = path;
            this.dataSerializer = dataSerializer;
            this.fileSystem = fileSystem;
        }

        public void Write(IEnumerable<DocumentCollection<T>> storage)
        {
            int index = 0;
            foreach (var collection in storage)
            {
                var collectionPath = Path.Combine(path, $"wiki{index++}.zip");
                SaveCollection(collection, collectionPath);
            }
        }

        private void SaveCollection(DocumentCollection<T> collection, string path)
        {
            using var stream = fileSystem.File.OpenWrite(path);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            WriteMetadata(archive, collection.Metadata);
            SaveDocuments(archive, collection.Documents);
        }

        private void WriteMetadata(ZipArchive archive, IDictionary<Guid, DocumentProperties> metadata)
        {
            string json = JsonConvert.SerializeObject(metadata, Formatting.Indented);
            ZipArchiveEntry contentsEntry = archive.CreateEntry(METADATA_ENTRY_NAME);
            using (StreamWriter writer = new StreamWriter(contentsEntry.Open()))
            {
                writer.Write(json);
            }
        }

        private void SaveDocuments(ZipArchive archive, IEnumerable<Document<T>> docs)
        {
            foreach (var doc in docs)
            {
                var entry = archive.CreateEntry(doc.Id.ToString() + ".txt");
                using var stream = entry.Open();
                dataSerializer.Serialize(stream, doc.Data);
            }
        }

    }
}
