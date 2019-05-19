﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wikipedia;

using static MoreLinq.Extensions.BatchExtension;

namespace Wikidump
{
    public class WikipediaReader : IWikipediaReader
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

        public IEnumerable<WikiCollection> Read()
        {
            return reader.ReadPages().Where(filter).Take(count).Batch(collectionSize).Select(ToCollection);
        }

        private static WikiCollection ToCollection(IEnumerable<WikiDumpPage> dumpPages)
        {
            var pages = dumpPages.Select(ToWikiPage).ToList();

            return new WikiCollection
            {
                Contents = BuildContents(pages),
                Pages = pages,
            };
        }

        private static WikiPage ToWikiPage(WikiDumpPage dp)
        {
            return new WikiPage
            {
                Id = Guid.NewGuid(),
                Title = dp.Title,
                Data = dp.Text,
            };
        }

        private static IDictionary<Guid, string> BuildContents(IList<WikiPage> pages)
        {
            return pages.ToDictionary(p => p.Id, p => p.Title);
        }
    }
}
