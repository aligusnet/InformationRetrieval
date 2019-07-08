using System;
using System.Collections.Generic;

namespace InformationRetrieval.Utility
{
    public static class TextHasher
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

        // this implementation repets previous one 
        // because there is no 'free' conversion from IList<T> to Span<T>
        public static int CalculateHashCode(IList<char> text, int from, int to)
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for (int i = from; i < to; i += 2)
            {
                int c = text[i];
                hash1 = ((hash1 << 5) + hash1) ^ c;
                if (i + 1 < to)
                {
                    c = text[i + 1];
                    hash2 = ((hash2 << 5) + hash2) ^ c;
                }
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
}
