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
        private readonly Stream postingsStream;
        public IDictionary<T, long> Offsets { get; }

        public ExternalIndexComposer(Stream postingsStream)
        {
            this.postingsStream = postingsStream;
            Offsets = new Dictionary<T, long>();
        }

        public long AddAllDocuments(IReadOnlyCollection<DocumentId> allDocs)
        {
            long position = postingsStream.Position;
            NaivePostingsSerializer.Serialize(postingsStream, allDocs);
            return position;
        }

        public long AddPostingsList(T term, IReadOnlyCollection<DocumentId> postingsList)
        {
            long position = postingsStream.Position;
            Offsets.Add(term, postingsStream.Position);
            NaivePostingsSerializer.Serialize(postingsStream, postingsList);
            return position;
        }

        public ExternalIndex<T> Compose()
        {
            postingsStream.Flush();
            postingsStream.Seek(0, SeekOrigin.Begin);

            return new ExternalIndex<T>(Offsets, postingsStream);
        }
    }
}
