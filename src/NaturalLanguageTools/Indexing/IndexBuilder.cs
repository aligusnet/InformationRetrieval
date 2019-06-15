using System.Collections.Generic;

using DocumentStorage;

namespace NaturalLanguageTools.Indexing
{
    public class IndexBuilder<TWord, TSequence> where TSequence : IEnumerable<TWord>
    {
        public IBuildableIndex<TWord> Index { get; }

        public IndexBuilder(IBuildableIndex<TWord> index)
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
            foreach (var word in doc.Data)
            {
                Index.IndexWord(doc.Metadata.Id, word, position++);
            }
        }
    }
}
