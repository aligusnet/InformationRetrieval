using System;
using System.Collections.Generic;
using System.IO;
using Corpus;

namespace InformationRetrieval.Indexing
{
    class DictonaryBasedExternalBuildableIndex<T> : IExternalBuildableIndex<T>
    {
        private readonly Stream postingsStream;
        private List<DocumentId> allDocuments;
        private Dictionary<T, List<DocumentId>> postingsLists;

        public DictonaryBasedExternalBuildableIndex(Stream postingsStream)
        {
            allDocuments = new List<DocumentId>();
            postingsLists = new Dictionary<T, List<DocumentId>>();
            this.postingsStream = postingsStream;
        }

        public ISearchableIndex<T> Build()
        {
            return BuildExternalIndex();
        }

        public ExternalIndex<T> BuildExternalIndex()
        {
            var composer = new ExternalIndexComposer<T>(postingsStream);

            composer.AddAllDocuments(allDocuments);

            foreach (var postings in postingsLists)
            {
                composer.AddPostingsList(postings.Key, postings.Value);
            }

            return composer.Compose();
        }

        public void IndexTerm(DocumentId id, T term, int position)
        {
            Index last = ^1;

            if (allDocuments.Count == 0 || allDocuments[last] != id)
            {
                allDocuments.Add(id);
            }

            if (!postingsLists.TryGetValue(term, out var postings))
            {
                postings = new List<DocumentId>();
                postings.Add(id);
                postingsLists.Add(term, postings);
            }
            else if (postings[last] != id)
            {
                postings.Add(id);
            }
        }
    }
}
