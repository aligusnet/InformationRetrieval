using System.Collections.Generic;

using Corpus;

namespace InformationRetrieval.Indexing
{
    public class IndexBuilder<TTerm, TSequence> 
        where TTerm : notnull
        where TSequence : IEnumerable<TTerm>
    {
        public IBuildableIndex<TTerm> Index { get; }

        public IndexBuilder(IBuildableIndex<TTerm> index)
        {
            Index = index;
        }

        public void IndexCorpus(IEnumerable<Block<TSequence>> corpus)
        {
            foreach (var block in corpus)
            {
                IndexBlock(block);
            }
        }

        public void IndexBlock(Block<TSequence> block)
        {
            foreach (var doc in block.Documents)
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
