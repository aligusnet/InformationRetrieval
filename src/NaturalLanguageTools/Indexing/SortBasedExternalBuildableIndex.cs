using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corpus;

namespace NaturalLanguageTools.Indexing
{
    public class SortBasedExternalBuildableIndex<T> : IBuildableIndex<T>
    {
        private readonly IList<(uint Id, T Term)> tokens;
        private readonly Stream postingsStream;

        public SortBasedExternalBuildableIndex(Stream postingsStream)
        {
            tokens = new List<(uint Id, T Term)>();
            this.postingsStream = postingsStream;
        }

        public ExternalIndex<T> Build()
        {
            var allDocs = tokens.Select(token => token.Id).Distinct().OrderBy(id => id);
            NaivePostingsSerializer.Serialize(postingsStream, allDocs.ToArray());

            var termsOffsets = new Dictionary<T, long>();
            var postingsList = from token in tokens
                               group token.Id by token.Term into postings
                               select postings;
            foreach (var postings in postingsList)
            {
                termsOffsets.Add(postings.Key, postingsStream.Position);
                NaivePostingsSerializer.Serialize(postingsStream, postings.OrderBy(id => id).Distinct().ToArray());
            }

            postingsStream.Flush();
            postingsStream.Seek(0, SeekOrigin.Begin);

            return new ExternalIndex<T>(termsOffsets, postingsStream);
        }

        public void IndexTerm(DocumentId docId, T term, int position)
        {
            tokens.Add((docId.Id, term));
        }

        ISearchableIndex<T> IBuildableIndex<T>.Build()
        {
            return Build();
        }
    }
}
