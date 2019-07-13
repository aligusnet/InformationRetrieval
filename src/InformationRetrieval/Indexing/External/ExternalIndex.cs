using System;
using System.Collections.Generic;
using System.IO;
using Corpus;
using InformationRetrieval.Indexing.PostingsList;

namespace InformationRetrieval.Indexing.External
{
    /// <summary>
    /// Index that stores data on disk
    /// </summary>
    public class ExternalIndex<T> : ISearchableIndex<T>, IDisposable
    {
        private const long AllDocumentsOffset = 0;
        public IDictionary<T, long> Offsets { get; }
        public Stream PostingsStream { get; }

        private readonly PostingsListReader reader;

        public ExternalIndex(IDictionary<T, long> offsets, Stream postingsStream)
        {
            Offsets = offsets;
            PostingsStream = postingsStream;
            reader = new PostingsListReader(postingsStream, leaveOpen: true);
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
            return reader.Read(offset);
        }

        private int ReadCount(long offset)
        {
            return reader.ReadCount(offset);
        }

        public void Dispose()
        {
            reader.Dispose();
            PostingsStream.Dispose();
        }
    }
}
