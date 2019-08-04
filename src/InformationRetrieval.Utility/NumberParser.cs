using System;

namespace InformationRetrieval.Utility
{
    public static class NumberParser
    {
        public static int ParseInt32(ReadOnlySpan<char> s, int fromBase)
        {
            var (sign, value) = ParseSigned(s, fromBase);
            return sign * (int)checked((uint)value);
        }

        public static ushort ParseUInt16(ReadOnlySpan<char> s, int fromBase)
        {
            return checked((ushort)ParseUnsigned(s, fromBase));
        }

        public static uint ParseUInt32(ReadOnlySpan<char> s, int fromBase)
        {
            return checked((uint)ParseUnsigned(s, fromBase));
        }

        private static (sbyte Sign, ulong Value) ParseSigned(ReadOnlySpan<char> s, int fromBase)
        {
            sbyte sign = 1;
            ulong unsignedValue;

            if (s[0] == '-')
            {
                sign = -1;
                unsignedValue = ParseUnsigned(s.Slice(1), fromBase);
            }
            else
            {
                unsignedValue = ParseUnsigned(s, fromBase);
            }

            return (sign, unsignedValue);
        }

        private static ulong ParseUnsigned(ReadOnlySpan<char> s, int fromBase)
        {
            ulong result = 0;

            for (int i = 0; i < s.Length; ++i)
            {
                char c = s[i];
                var d = GetDigit(c);
                if (d < 0 || d >= fromBase)
                {
                    throw new FormatException($"Unexpected character {c} at position {i}");
                }

                result *= (ulong)fromBase;
                result += (ulong)d;
            }

            return result;
        }

        private static int GetDigit(char c)
        {
            int digit = -1;

            if (c >= '0' && c <= '9')
            {
                digit = c - '0';
            }

            else if (c >= 'A' && c <= 'F')
            {
                digit = 10 + c - 'A';
            }

            else if (c >= 'a' && c <= 'f')
            {
                digit = 10 + c - 'a';
            }

            return digit;
        }
    }
}
