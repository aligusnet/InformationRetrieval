using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
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
        private readonly IFileSystem fileSystem;

        public StorageZipReader(string path, IDocumentDataSerializer<T> dataSerializer) : this (path, dataSerializer, new FileSystem())
        {
        }

        public StorageZipReader(string path, IDocumentDataSerializer<T> dataSerializer, IFileSystem fileSystem)
        {
            this.path = path;
            this.dataSerializer = dataSerializer;
            this.fileSystem = fileSystem;
        }

        public IEnumerable<DocumentCollection<T>> Read()
        {
            return Read(fileSystem.Directory.GetFiles(path, "*.zip"));
        }

        private IEnumerable<DocumentCollection<T>> Read(IEnumerable<string> archives)
        {
            foreach (var archivePath in archives)
            {
                using var stream = fileSystem.File.OpenRead(archivePath);
                using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

                yield return ReadArchive(archive);
            }
        }

        private DocumentCollection<T> ReadArchive(ZipArchive archive)
        {
            var metadata = ReadMetadata(archive);

            return new DocumentCollection<T>(ReadDocuments(archive, metadata).ToList(), metadata);
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
                    using var stream = entry.Open();

                    var data = dataSerializer.Deserialize(stream);
                    var id = Guid.Parse(Path.GetFileNameWithoutExtension(entry.Name));

                    yield return new Document<T>(id, metadata[id].Title, data);
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
