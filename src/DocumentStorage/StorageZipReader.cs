using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;

namespace DocumentStorage
{
    /// <summary>
    /// Implementation of Storage Reader
    /// </summary>
    public class StorageZipReader<T> : IStorageReader<T>
    {
        private const string METADATA_ENTRY_NAME = StorageZipWriter<T>.METADATA_ENTRY_NAME;

        private readonly string path;
        private readonly IDocumentDataSerializer<T> dataSerializer;

        public StorageZipReader(string path, IDocumentDataSerializer<T> dataSerializer)
        {
            this.path = path;
            this.dataSerializer = dataSerializer;
        }

        public IEnumerable<DocumentCollection<T>> Read()
        {
            return Read(Directory.GetFiles(path, "*.zip"));
        }

        private IEnumerable<DocumentCollection<T>> Read(IEnumerable<string> archives)
        {
            foreach (var archivePath in archives)
            {
                using (var archive = ZipFile.OpenRead(archivePath))
                {
                    yield return ReadArchive(archive);
                }
            }
        }

        private DocumentCollection<T> ReadArchive(ZipArchive archive)
        {
            var metadata = ReadMetadata(archive);

            return new DocumentCollection<T>
            {
                Metadata = metadata,
                Documents = ReadDocuments(archive, metadata).ToList(),
            };
        }

        private IDictionary<Guid, DocumentProperties> ReadMetadata(ZipArchive archive)
        {
            var entry = archive.GetEntry(METADATA_ENTRY_NAME);

            return JsonConvert.DeserializeObject<IDictionary<Guid, DocumentProperties>>(ReadStringZipEntry(entry));
        }

        private IEnumerable<Document<T>> ReadDocuments(ZipArchive archive, IDictionary<Guid, DocumentProperties> metadata)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.Name != METADATA_ENTRY_NAME)
                {
                    var data = dataSerializer.Deserialize(entry.Open());
                    var id = Guid.Parse(Path.GetFileNameWithoutExtension(entry.Name));

                    yield return new Document<T>
                    {
                        Id = id,
                        Title = metadata[id].Title,
                        Data = data,
                    };
                }
            }
        }

        protected string ReadStringZipEntry(ZipArchiveEntry entry)
        {
            using (var reader = new StreamReader(entry.Open()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
