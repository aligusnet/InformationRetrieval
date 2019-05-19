using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MoreLinq.Extensions.BatchExtension;
using static Wikidump.WikiPageCollectionName;

namespace Wikidump
{
    public abstract class WikiDumpWriter : IWikiDumpWriter
    {
        private readonly int collectionSize;
        private readonly IEnumerable<string> collectionNames;
        private readonly IList<WikiPageInfo> infoList = new List<WikiPageInfo>();

        protected const string ContentsFilename = "_CONTENTS_.tsv";

        public WikiDumpWriter(string patternCollectionPath, int collectionSize)
        {
            this.collectionSize = collectionSize;
            this.collectionNames = GenerateCollectionNames(patternCollectionPath);

            var path = Path.GetDirectoryName(patternCollectionPath);

            if (Directory.Exists(path))
            {
                var tmp = path + "old";
                Directory.Move(path, tmp);
                Directory.Delete(tmp, recursive: true);
            }

            Directory.CreateDirectory(path);
        }

        public void WritePages(IEnumerable<WikiPage> pages)
        {
            pages.Batch(collectionSize).Zip(
                collectionNames,
                (chunk, archiveName) =>
                {
                    WriteCollection(archiveName, chunk);
                    return true;
                }).All(b => b);
        }

        protected abstract void WriteCollection(string collectionPath, IEnumerable<WikiPage> pages);

        protected WikiPageInfo AddInfo(WikiPage page)
        {
            string filename = Guid.NewGuid().ToString() + ".txt";
            var info = new WikiPageInfo
            {
                Title = page.Title,
                FileName = filename
            };

            infoList.Add(info);

            return info;
        }

        protected void FlushInfoList(StreamWriter stream)
        {
            foreach (var info in infoList)
            {
                stream.WriteLine("{0}\t{1}", info.Title, info.FileName);
            }

            infoList.Clear();
        }
    }
}
