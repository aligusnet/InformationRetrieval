using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentStorage;

namespace NaturalLanguageTools.Indexing
{
    /// <summary>
    /// Index that stores data on disk
    /// </summary>
    public class ExternalIndex<T> : ISearchableIndex<T>
    {
        private const long AllDocumentsOffset = 0;
        private readonly IDictionary<T, long> offsets = new Dictionary<T, long>();
        private readonly Stream postingsStream;

        public ExternalIndex(IDictionary<T, long> offsets, Stream postingsStream)
        {
            this.offsets = offsets;
            this.postingsStream = postingsStream;
        }

        public IEnumerable<DocumentId> GetAll()
        {
            return ReadPostings(AllDocumentsOffset);
        }

        public int GetCount(T word)
        {
            if (offsets.TryGetValue(word, out long offset))
            {
                return ReadCount(offset);
            }

            return 0;
        }

        public int GetCount()
        {
            return ReadCount(AllDocumentsOffset);
        }

        public IEnumerable<DocumentId> Search(T word)
        {
            if (offsets.TryGetValue(word, out long offset))
            {
                return ReadPostings(offset);
            }

            return Array.Empty<DocumentId>();
        }

        private IEnumerable<DocumentId> ReadPostings(long offset)
        {
            postingsStream.Seek(offset, SeekOrigin.Begin);
            return NaivePostingsSerializer.Deserialize(postingsStream).Select(id => new DocumentId(id));
        }

        private int ReadCount(long offset)
        {
            postingsStream.Seek(offset, SeekOrigin.Begin);
            return NaivePostingsSerializer.DeserializeCount(postingsStream);
        }
    }
}
