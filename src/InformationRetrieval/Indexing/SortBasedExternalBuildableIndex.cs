using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corpus;

namespace InformationRetrieval.Indexing
{
    public class SortBasedExternalBuildableIndex<T> : IExternalBuildableIndex<T>
    {
        private readonly IList<(DocumentId Id, T Term)> tokens;
        private readonly Stream postingsStream;

        public SortBasedExternalBuildableIndex(Stream postingsStream)
        {
            tokens = new List<(DocumentId Id, T Term)>();
            this.postingsStream = postingsStream;
        }

        public ExternalIndex<T> BuildExternalIndex()
        {
            var composer = new ExternalIndexComposer<T>(postingsStream);

            var allDocs = tokens.Select(token => token.Id).Distinct().OrderBy(id => id);
            composer.AddAllDocuments(allDocs.ToArray());

            var postingsList = from token in tokens
                               group token.Id by token.Term into postings
                               select postings;
            foreach (var postings in postingsList)
            {
                composer.AddPostingsList(postings.Key, postings.OrderBy(id => id).Distinct().ToArray());
            }

            return composer.Compose();
        }

        public void IndexTerm(DocumentId docId, T term, int position)
        {
            tokens.Add((docId, term));
        }

        public ISearchableIndex<T> Build()
        {
            return BuildExternalIndex();
        }
    }
}
