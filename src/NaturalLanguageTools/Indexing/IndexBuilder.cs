using System.Collections.Generic;

using DocumentStorage;

namespace NaturalLanguageTools.Indexing
{
    public class IndexBuilder<TTerm, TSequence> where TSequence : IEnumerable<TTerm>
    {
        public IBuildableIndex<TTerm> Index { get; }

        public IndexBuilder(IBuildableIndex<TTerm> index)
        {
            Index = index;
        }

        public void IndexStorage(IEnumerable<DocumentCollection<TSequence>> storage)
        {
            foreach (var collection in storage)
            {
                IndexCollection(collection);
            }
        }

        public void IndexCollection(DocumentCollection<TSequence> collection)
        {
            foreach (var doc in collection.Documents)
            {
                IndexDocument(doc);
            }
        }

        public void IndexDocument(Document<TSequence> doc)
        {
            int position = 0;
            foreach (var term in doc.Data)
            {
                Index.IndexTerm(doc.Metadata.Id, term, position++);
            }
        }

        public ISearchableIndex<TTerm> Build()
        {
            return Index.Build();
        }
    }
}
