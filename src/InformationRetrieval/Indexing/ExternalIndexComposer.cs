using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Corpus;

namespace InformationRetrieval.Indexing
{
    /// <summary>
    ///  Compose External Index using precomputed postings lists.
    /// </summary>
    public class ExternalIndexComposer<T>
    {
        private readonly PostingsListWriter writer;
        public IDictionary<T, long> Offsets { get; }

        public ExternalIndexComposer(Stream postingsStream)
        {
            writer = new PostingsListWriter(postingsStream);
            Offsets = new Dictionary<T, long>();
        }

        public long AddAllDocuments(IReadOnlyCollection<DocumentId> allDocs)
        {
            return writer.Write(allDocs);
        }

        public long AddPostingsList(T term, IReadOnlyCollection<DocumentId> postingsList)
        {
            long position = writer.Write(postingsList);
            Offsets.Add(term, position);
            return position;
        }

        public ExternalIndex<T> Compose()
        {
            return new ExternalIndex<T>(Offsets, writer.Reset());
        }
    }
}