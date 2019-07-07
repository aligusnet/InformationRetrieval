using System;
using System.Collections.Generic;
using System.IO;
using Corpus;

namespace NaturalLanguageTools.Indexing
{
    /// <summary>
    /// Index that stores data on disk
    /// </summary>
    public class ExternalIndex<T> : ISearchableIndex<T>, IDisposable
    {
        private const long AllDocumentsOffset = 0;
        public IDictionary<T, long> Offsets { get; }
        public Stream PostingsStream { get; }

        public ExternalIndex(IDictionary<T, long> offsets, Stream postingsStream)
        {
            this.Offsets = offsets;
            this.PostingsStream = postingsStream;
        }

        public IEnumerable<DocumentId> GetAll()
        {
            return ReadPostings(AllDocumentsOffset);
        }

        public int GetCount(T word)
        {
            if (Offsets.TryGetValue(word, out long offset))
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
            if (Offsets.TryGetValue(word, out long offset))
            {
                return ReadPostings(offset);
            }

            return Array.Empty<DocumentId>();
        }

        private IEnumerable<DocumentId> ReadPostings(long offset)
        {
            PostingsStream.Seek(offset, SeekOrigin.Begin);
            return NaivePostingsSerializer.Deserialize(PostingsStream);
        }

        private int ReadCount(long offset)
        {
            PostingsStream.Seek(offset, SeekOrigin.Begin);
            return NaivePostingsSerializer.DeserializeCount(PostingsStream);
        }

        public void Dispose()
        {
            this.PostingsStream.Dispose();
        }
    }
}
