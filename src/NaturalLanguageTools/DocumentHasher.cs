using System;
using System.Collections.Generic;
using System.Linq;

using NaturalLanguageTools.Transformers;

namespace NaturalLanguageTools
{
    public class DocumentHasher : StorageTransformer<IEnumerable<string>, int[]>
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
