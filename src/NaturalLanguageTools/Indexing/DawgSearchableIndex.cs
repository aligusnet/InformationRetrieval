using System.Linq;
using System.Collections.Generic;

using DawgSharp;
using DocumentStorage;

namespace NaturalLanguageTools.Indexing
{
    public class DawgSearchableIndex : ISearchableIndex<string>
    {
        private readonly Dawg<DocumentIdRangeCollectionList> dawg;
        private readonly DocumentIdRangeCollectionList allDocuments;

        public DawgSearchableIndex(Dawg<DocumentIdRangeCollectionList> dawg, DocumentIdRangeCollectionList allDocuments)
        {
            this.dawg = dawg;
            this.allDocuments = allDocuments;
        }

        public IEnumerable<DocumentId> AllDocuments()
        {
            return allDocuments;
        }

        public IEnumerable<DocumentId> Search(string word)
        {
            return dawg[word] ?? Enumerable.Empty<DocumentId>();
        }
    }
}
