using System;
using System.Collections.Generic;
using System.Linq;
using DocumentStorage;

using static MoreLinq.Extensions.BatchExtension;

namespace Wikidump
{
    public class WikipediaReader : IStorageReader<string>
    {
        public static readonly Func<WikiDumpPage, bool> DefaultFilter = p => p.IsContent;

        private readonly IWikiDumpReader reader;
        private readonly Func<WikiDumpPage, bool> filter;
        private readonly int collectionSize;
        private readonly int count;

        public WikipediaReader(IWikiDumpReader reader, Func<WikiDumpPage, bool> filter, int collectionSize = 1000, int count = int.MaxValue)
        {
            this.reader = reader;
            this.filter = filter;
            this.collectionSize = collectionSize;
            this.count = count;
        }

        public WikipediaReader(IWikiDumpReader reader) : this(reader, DefaultFilter)
        {
        }

        public IEnumerable<DocumentCollection<string>> Read()
        {
            return reader.ReadPages().Where(filter).Take(count).Batch(collectionSize).Select(ToCollection);
        }

        private static DocumentCollection<string> ToCollection(IEnumerable<WikiDumpPage> dumpPages)
        {
            var docs = dumpPages.Select(ToDocument).ToList();

            return DocumentCollection<string>.Make(docs);
        }

        private static Document<string> ToDocument(WikiDumpPage dp)
        {
            return new Document<string>(Guid.NewGuid(), dp.Title, dp.Text);
        }
    }
}
