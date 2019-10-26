using System.Collections.Generic;

using Corpus;

namespace InformationRetrieval.Indexing.PostingsList
{
    /// <summary>
    /// Builds postings lists using mixed strategy: 
    /// ranged postings lists for frequent terms
    /// and uncompressed postings list otherwise.
    /// </summary>
    /// <typeparam name="T">Term's type</typeparam>
    public class MixedPostingsListBuilder<T> where T : notnull
    {
        private readonly int rangeThreshold;
        public RangePostingsList AllDocuments;
        public IDictionary<T, RangePostingsList> RangedPostingsLists;
        public IDictionary<T, List<DocumentId>> UncompressedPostingsLists;

        public MixedPostingsListBuilder(int rangeThreshold)
        {
            AllDocuments = new RangePostingsList();
            RangedPostingsLists = new Dictionary<T, RangePostingsList>();
            UncompressedPostingsLists = new Dictionary<T, List<DocumentId>>();
            this.rangeThreshold = rangeThreshold;
        }

        public void Add(DocumentId id, T word)
        {
            if (RangedPostingsLists.TryGetValue(word, out var blockList))
            {
                blockList.Add(id);
            }
            else
            {
                if (!UncompressedPostingsLists.TryGetValue(word, out var ids))
                {
                    ids = new List<DocumentId>(1);
                    UncompressedPostingsLists.Add(word, ids);
                    ids.Add(id);
                }
                else if (ids[^1] != id)
                {
                    ids.Add(id);
                }

                int count = ids.Count;
                if (count >= rangeThreshold && id.Id - ids[count - rangeThreshold].Id == (rangeThreshold - 1))
                {
                    MoveToIndex(word, ids);
                }
            }

            AllDocuments.Add(id);
        }

        private void MoveToIndex(T word, IList<DocumentId> ids)
        {
            UncompressedPostingsLists.Remove(word);

            var list = new RangePostingsList();
            foreach (var id in ids)
            {
                list.Add(id);
            }

            RangedPostingsLists.Add(word, list);
        }
    }
}
