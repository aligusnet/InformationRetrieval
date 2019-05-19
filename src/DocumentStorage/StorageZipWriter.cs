using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace DocumentStorage
{
    /// <summary>
    /// Implementation of Storage Writer
    /// </summary>
    public class StorageZipWriter : IStorageWriter
    {
        internal const string METADATA_ENTRY_NAME = "_METADATA_.json";

        private readonly string path;

        public StorageZipWriter(string path)
        {
            this.path = path;
        }

        public void Write(IEnumerable<DocumentCollection> storage)
        {
            int index = 0;
            foreach (var collection in storage)
            {
                var collectionPath = Path.Combine(path, $"wiki{index++}.zip");
                SaveCollection(collection, collectionPath);
            }
        }

        private void SaveCollection(DocumentCollection collection, string path)
        {
            using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
            {
                WriteMetadata(archive, collection.Metadata);
                SavePages(archive, collection.Pages);
            }
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

        private void SavePages(ZipArchive archive, IEnumerable<Document> pages)
        {
            foreach (var page in pages)
            {
                var entry = archive.CreateEntry(page.Id.ToString() + ".txt");
                using (var stream = new StreamWriter(entry.Open()))
                {
                    stream.Write(page.Data);
                }
            }
        }
    }
}
