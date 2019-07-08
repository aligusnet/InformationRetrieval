using System.Linq;
using Corpus;

namespace InformationRetrieval.Transformers
{
    class BlockTransformer<TIn, TOut> : IBlockTransformer<TIn, TOut>
    {
        private readonly IDocumentTransformer<TIn, TOut> documentTransformer;

        public BlockTransformer(IDocumentTransformer<TIn, TOut> documentTransformer)
        {
            this.documentTransformer = documentTransformer;
        }

        public Block<TOut> Transform(Block<TIn> source)
        {
            var target = source.Documents.Select(d => documentTransformer.Transform(d)).ToList();
            return Block<TOut>.Make(source.Metadata.Id, target);
        }
    }
}
