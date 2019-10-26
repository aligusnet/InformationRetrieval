using Corpus;
using System;
using System.Collections.Generic;
using System.Text;

namespace InformationRetrieval.Indexing.PostingsList
{
    public interface IPostingsListBuilder<T> where T : notnull
    {
        void AddTerm(DocumentId id, T term);

        IReadOnlyCollection<DocumentId> Documents { get; }
        IEnumerable<KeyValuePair<T, IReadOnlyCollection<DocumentId>>> PostingsLists { get; }
    }
}
