using System;
using System.Collections.Generic;
using System.Linq;
using DocumentStorage;

using static MoreLinq.Extensions.BatchExtension;

namespace Wikidump
{
    public class WikipediaReader : IStorageReader
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

        public IEnumerable<DocumentCollection> Read()
        {
            return reader.ReadPages().Where(filter).Take(count).Batch(collectionSize).Select(ToCollection);
        }

        private static DocumentCollection ToCollection(IEnumerable<WikiDumpPage> dumpPages)
        {
            var pages = dumpPages.Select(ToWikiPage).ToList();

            return new DocumentCollection
            {
                Contents = BuildContents(pages),
                Pages = pages,
            };
        }

        private static Document ToWikiPage(WikiDumpPage dp)
        {
            return new Document
            {
                Id = Guid.NewGuid(),
                Title = dp.Title,
                Data = dp.Text,
            };
        }

        private static IDictionary<Guid, string> BuildContents(IList<Document> pages)
        {
            return pages.ToDictionary(p => p.Id, p => p.Title);
        }
    }
}
