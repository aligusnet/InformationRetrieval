using System;
using System.Collections.Generic;
using System.Linq;

using NaturalLanguageTools.Transformers;

namespace NaturalLanguageTools
{
    public class DocumentHasher : StorageTransformer<IEnumerable<string>, int[]>
    {
        public static int CalculateHashCode(ReadOnlySpan<char> s)
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = 0; i < s.Length; i += 2)
            {
                int c = s[i];
                hash1 = ((hash1 << 5) + hash1) ^ c;
                if (i + 1 < s.Length)
                {
                    c = s[i + 1];
                    hash2 = ((hash2 << 5) + hash2) ^ c;
                }
            }

            return hash1 + (hash2 * 1566083941);
        }

        public DocumentHasher() : base(TransformData)
        {
        }

        private static int[] TransformData(IEnumerable<string> words)
        {
            return words.Select(w => CalculateHashCode(w.AsSpan())).ToArray();
        }
    }
}
