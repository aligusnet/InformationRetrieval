using System.Collections.Generic;
using System.IO;

namespace Wikidump
{
    public class WikiDumpFileWriter: WikiDumpWriter
    {
        public WikiDumpFileWriter(string pathToSave, int collectionSize = 1000, string dirNamePrefix = "wiki") :
            base(Path.Combine(pathToSave, dirNamePrefix + "{0}"), collectionSize)
        {
        }

        protected override void WriteCollection(string collectionPath, IEnumerable<WikiPage> pages)
        {
            Directory.CreateDirectory(collectionPath);
            foreach (var page in pages)
            {
                string filename = AddInfo(page).FileName;
                File.WriteAllText(Path.Combine(collectionPath, filename), page.Text);
            }

            using (var stream = File.CreateText(Path.Combine(collectionPath, ContentsFilename)))
            {
                FlushInfoList(stream);
            }
        }
    }
}
