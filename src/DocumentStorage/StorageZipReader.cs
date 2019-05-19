using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace DocumentStorage
{
    /// <summary>
    /// Implementation of Storage Reader
    /// </summary>
    public class StorageZipReader : IStorageReader
    {
        private const string CONTENT_ENTRY_NAME = StorageZipWriter.CONTENT_ENTRY_NAME;

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
            var content = ReadContent(archive);

            return new DocumentCollection
            {
                Contents = content,
                Pages = ReadPages(archive, content),
            };
        }

        private IDictionary<Guid, string> ReadContent(ZipArchive archive)
        {
            var entry = archive.GetEntry(CONTENT_ENTRY_NAME);

            var content = new Dictionary<Guid, string>();

            using (var reader = new StreamReader(entry.Open()))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine().Split('\t');
                    content.Add(Guid.Parse(Path.GetFileNameWithoutExtension(line[1])), line[0]);
                }
            }

            return content;
        }

        private IEnumerable<Document> ReadPages(ZipArchive archive, IDictionary<Guid, string> content)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.Name != CONTENT_ENTRY_NAME)
                {
                    var data = ReadZipEntry(entry);
                    var id = Guid.Parse(Path.GetFileNameWithoutExtension(entry.Name));

                    yield return new Document
                    {
                        Id = id,
                        Title = content[id],
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
