using static System.Threading.Tasks.Parallel;
using Corpus;

namespace InformationRetrieval.Transformers.Parallel
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
            var target = new Document<TOut>[source.Documents.Count];
            For(0, target.Length, index => target[index] = documentTransformer.Transform(source.Documents[index]));
            return Block<TOut>.Make(source.Metadata.Id, target);
        }
    }
}
