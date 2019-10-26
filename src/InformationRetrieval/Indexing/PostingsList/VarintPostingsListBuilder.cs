using System;
using System.Collections.Generic;
using Corpus;

namespace InformationRetrieval.Indexing.PostingsList
{
    /// <summary>
    /// Builds postings lists using VarInt compression
    /// </summary>
    /// <typeparam name="T">Term's type</typeparam>
    public class VarintPostingsListBuilder<T> : IPostingsListBuilder<T> where T : notnull
    {
        private readonly IDictionary<T, VarintPostingsList> postingsLists = new Dictionary<T, VarintPostingsList>();
        private readonly VarintPostingsList documents = new VarintPostingsList();

        public IReadOnlyCollection<DocumentId> Documents => documents;

        public IEnumerable<KeyValuePair<T, IReadOnlyCollection<DocumentId>>> PostingsLists
        {
            get
            {
                foreach (var postings in postingsLists)
                {
                    yield return new KeyValuePair<T, IReadOnlyCollection<DocumentId>>(postings.Key, postings.Value);
                }
            }
        }

        public void AddTerm(DocumentId id, T term)
        {
            if (!postingsLists.TryGetValue(term, out var ps))
            {
                ps = new VarintPostingsList();
                postingsLists.Add(term, ps);
            }

            ps.Add(id);
            documents.Add(id);
        }

        public IDictionary<T, VarintPostingsList> VarintPostingsLists => postingsLists;
    }
}
