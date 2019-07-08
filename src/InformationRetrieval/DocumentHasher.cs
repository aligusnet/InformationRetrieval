using System;
using System.Collections.Generic;
using System.Linq;

using InformationRetrieval.Transformers;

namespace InformationRetrieval
{
    public class DocumentHasher : CorpusTransformer<IEnumerable<string>, int[]>
    {
        public DocumentHasher() : base(TransformData)
        {
        }

        private static int[] TransformData(IEnumerable<string> words)
        {
            return words.Select(w => Utility.TextHasher.CalculateHashCode(w.AsSpan())).ToArray();
        }
    }
}
