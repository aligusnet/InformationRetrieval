using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DocumentStorage
{
    /// <summary>
    /// Implementation of Storage Writer
    /// </summary>
    public class StorageZipWriter : IStorageWriter
    {
        internal const string CONTENT_ENTRY_NAME = "_CONTENTS_.tsv";

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
                SaveContent(archive, collection.Contents);
                SavePages(archive, collection.Pages);
            }
        }

        private void SaveContent(ZipArchive archive, IDictionary<Guid, string> content)
        {
            ZipArchiveEntry contentsEntry = archive.CreateEntry(CONTENT_ENTRY_NAME);
            using (StreamWriter writer = new StreamWriter(contentsEntry.Open()))
            {
                foreach (var entry in content)
                {
                    writer.WriteLine($"{entry.Value}\t{entry.Key}.txt");
                }

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
