using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Wikidump
{
    public class WikiDumpZipWriter : WikiDumpWriter
    {
        public WikiDumpZipWriter(string pathToSave, int collectionSize = 1000, string archiveNamePrefix = "wiki") : 
            base(Path.Combine(pathToSave, archiveNamePrefix + "{0}" + ".zip"), collectionSize)
        {
        }

        protected override void WriteCollection(string collectionPath, IEnumerable<WikiPage> pages)
        {
            var infos = new List<WikiPageInfo>();
            using (var zipToOpen = new FileStream(collectionPath, FileMode.CreateNew))
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                foreach (var page in pages)
                {
                    string filename = AddInfo(page).FileName;

                    ZipArchiveEntry entry = archive.CreateEntry(filename);
                    using (StreamWriter writer = new StreamWriter(entry.Open()))
                    {
                        writer.Write(page.Text);
                    }
                }

                ZipArchiveEntry contentsEntry = archive.CreateEntry(ContentsFilename);
                using (StreamWriter writer = new StreamWriter(contentsEntry.Open()))
                {
                    FlushInfoList(writer);
                }

            }
        }
    }
}
