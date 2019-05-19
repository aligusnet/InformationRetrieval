using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace DocumentStorage
{
    /// <summary>
    /// Implementation of Storage Reader
    /// </summary>
    public class StorageZipReader : IStorageReader
    {
        private const string METADATA_ENTRY_NAME = StorageZipWriter.METADATA_ENTRY_NAME;

        private readonly string path;

        public StorageZipReader(string path)
        {
            this.path = path;
        }

        public IEnumerable<DocumentCollection> Read()
        {
            return Read(Directory.GetFiles(path, "*.zip"));
        }

        private IEnumerable<DocumentCollection> Read(IEnumerable<string> archives)
        {
            foreach (var archivePath in archives)
            {
                using (var archive = ZipFile.OpenRead(archivePath))
                {
                    yield return ReadArchive(archive);
                }
            }
        }

        private DocumentCollection ReadArchive(ZipArchive archive)
        {
            var metadata = ReadMetadata(archive);

            return new DocumentCollection
            {
                Metadata = metadata,
                Pages = ReadDocuments(archive, metadata),
            };
        }

        private IDictionary<Guid, DocumentProperties> ReadMetadata(ZipArchive archive)
        {
            var entry = archive.GetEntry(METADATA_ENTRY_NAME);

            var content = new Dictionary<Guid, string>();

            return JsonConvert.DeserializeObject<IDictionary<Guid, DocumentProperties>>(ReadZipEntry(entry));
        }

        private IEnumerable<Document> ReadDocuments(ZipArchive archive, IDictionary<Guid, DocumentProperties> metadata)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.Name != METADATA_ENTRY_NAME)
                {
                    var data = ReadZipEntry(entry);
                    var id = Guid.Parse(Path.GetFileNameWithoutExtension(entry.Name));

                    yield return new Document
                    {
                        Id = id,
                        Title = metadata[id].Title,
                        Data = data,
                    };
                }
            }
        }

        private string ReadZipEntry(ZipArchiveEntry entry)
        {
            using (var reader = new StreamReader(entry.Open()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
