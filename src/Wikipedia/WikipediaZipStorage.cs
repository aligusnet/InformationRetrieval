using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Wikipedia
{
    /// <summary>
    /// Implementation of Wikipedia Storage
    /// </summary>
    public class WikipediaZipStorage : IWikipediaReader, IWikipediaWriter
    {
        private const string CONTENT_ENTRY_NAME = "_CONTENTS_.tsv";

        public IEnumerable<WikiCollection> Read(string path)
        {
            return Read(Directory.GetFiles(path, "*.zip"));
        }

        public void Write(IEnumerable<WikiCollection> wikipedia, string path)
        {
            int index = 0;
            foreach (var collection in wikipedia)
            {
                var collectionPath = Path.Combine(path, $"wiki{index++}.zip");
                SaveCollection(collection, collectionPath);
            }
        }

        private IEnumerable<WikiCollection> Read(IEnumerable<string> archives)
        {
            foreach (var archivePath in archives)
            {
                using (var archive = ZipFile.OpenRead(archivePath))
                {
                    yield return ReadArchive(archive);
                }
            }
        }

        private WikiCollection ReadArchive(ZipArchive archive)
        {
            var content = ReadContent(archive);

            return new WikiCollection
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

        private IEnumerable<WikiPage> ReadPages(ZipArchive archive, IDictionary<Guid, string> content)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.Name != CONTENT_ENTRY_NAME)
                {
                    var data = ReadZipEntry(entry);
                    var id = Guid.Parse(Path.GetFileNameWithoutExtension(entry.Name));

                    yield return new WikiPage
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

        private void SaveCollection(WikiCollection collection, string path)
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

        private void SavePages(ZipArchive archive, IEnumerable<WikiPage> pages)
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
