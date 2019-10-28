using System;
using System.Collections.Generic;
using System.Linq;
using Corpus;

using static MoreLinq.Extensions.BatchExtension;

namespace Wikidump
{
    public class WikipediaReader : ICorpusReader<string>
    {
        public static readonly Func<WikiDumpPage, bool> DefaultFilter = p => p.IsContent;

        private readonly IWikiDumpReader reader;
        private readonly Func<WikiDumpPage, bool> filter;
        private readonly ushort blockSize;
        private readonly int count;

        public WikipediaReader(IWikiDumpReader reader, Func<WikiDumpPage, bool> filter, ushort blockSize = 1000, int count = int.MaxValue)
        {
            this.reader = reader;
            this.filter = filter;
            this.blockSize = blockSize;
            this.count = count;
        }

        public WikipediaReader(IWikiDumpReader reader) : this(reader, DefaultFilter)
        {
        }

        public IEnumerable<Block<string>> Read()
        {
            return reader.ReadPages().Where(filter).Take(count).Batch(blockSize).Select(ToBlock);
        }

        private Block<string> ToBlock(IEnumerable<WikiDumpPage> dumpPages, int blockId)
        {
            uint startId = (uint)(blockId * blockSize);
            var docs = dumpPages.Select((dp, lid) => ToDocument(dp, startId + (uint)lid)).ToList();
            return Block<string>.Make((ushort)blockId, docs);
        }

        private Document<string> ToDocument(WikiDumpPage dp, uint id)
        {
            var metadata = new DocumentMetadata(new DocumentId(id), dp.Title);
            return new Document<string>(metadata, dp.Text);
        }
    }
}
