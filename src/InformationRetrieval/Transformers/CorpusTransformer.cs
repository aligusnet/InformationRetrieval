using System;
using System.Collections.Generic;
using System.Linq;
using Corpus;

namespace InformationRetrieval.Transformers
{
    public class CorpusTransformer<TIn, TOut> : ICorpusTransformer<TIn, TOut>
    {
        private readonly IBlockTransformer<TIn, TOut> blockTransformer;

        public CorpusTransformer(Func<TIn, TOut> processData) : this(new DocumentTransformer<TIn, TOut>(processData))
        {
        }

        public CorpusTransformer(IDocumentTransformer<TIn, TOut> documentTransformer) : this(new BlockTransformer<TIn, TOut>(documentTransformer))
        {
        }

        public CorpusTransformer(IBlockTransformer<TIn, TOut> blockTransformer)
        {
            this.blockTransformer = blockTransformer;
        }

        public IEnumerable<Block<TOut>> Transform(IEnumerable<Block<TIn>> source)
        {
            return source.Select(dc => blockTransformer.Transform(dc));
        }

        public void Transform(ICorpusReader<TIn> reader, ICorpusWriter<TOut> writer)
        {
            var source = reader.Read();
            var target = Transform(source);
            writer.Write(target);
        }
    }
}
