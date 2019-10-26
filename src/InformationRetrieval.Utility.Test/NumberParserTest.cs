using System;
using Xunit;

using InformationRetrieval.Utility;

namespace InformationRetrieval.Utility.Test
{
    public class NumberParserTest
    {
        [Fact]
        public void ParseIntTest()
        {
            var numbers = new int[] { 1, 20, 100, 200, 3000, -200, -30000 };

            foreach (var number in numbers)
            {
                Assert.Equal(number, NumberParser.ParseInt32(ToHex(number).AsSpan(), 16));
            }
        }

        [Fact]
        public void ParseUInt16Test()
        {
            var numbers = new ushort[] { 1, 20, 100, 200, 3000 };

            foreach (var number in numbers)
            {
                Assert.Equal(number, NumberParser.ParseUInt16(ToHex(number).AsSpan(), 16));
            }
        }

        [Fact]
        public void ParseUInt32Test()
        {
            var numbers = new uint[] { 1, 20, 100, 200, 3000 };

            foreach (var number in numbers)
            {
                Assert.Equal(number, NumberParser.ParseUInt32(ToHex(number).AsSpan(), 16));
            }
        }

        [Theory]
        [InlineData("00112", 2)]
        [InlineData("123a", 10)]
        [InlineData("Af1@", 16)]
        public void IncorrectFormatTest(string input, int fromBase)
        {
            Assert.Throws<FormatException>(() => NumberParser.ParseUInt32(input.AsSpan(), fromBase));
        }

        [Fact]
        public void OverflowUInt16Test()
        {
            Assert.Throws<OverflowException>(() => NumberParser.ParseUInt16("100000", 10));
        }

        [Fact]
        public void OverflowInt32Test()
        {
            long val = 1 + (long)int.MaxValue;
            Assert.Throws<OverflowException>(() => NumberParser.ParseUInt16(ToHex(val).AsSpan(), 16));
        }

        private static string ToHex<T>(T h)
        {
            return string.Format($"{h:X8}");
        }
    }
}
