using System;
using System.Linq;

using Xunit;
using InformationRetrieval.Utility;

namespace InformationRetrieval.Test.Utility
{
    public class VarintEncoderTest
    {
        [Fact]
        public void VarintEncoderSmokeTest()
        {
            var data = new byte[30];
            int pos = 0;
            pos += VarintEncoder.Encode(1023, data.AsSpan(pos));
            pos += VarintEncoder.Encode(5, data.AsSpan(pos));
            pos += VarintEncoder.Encode(ulong.MaxValue, data.AsSpan(pos));
            pos += VarintEncoder.Encode(ulong.MinValue, data.AsSpan(pos));

            var encoded = VarintEncoder.Decode(data, 0, pos).ToArray();

            Assert.Equal(new ulong[] { 1023, 5, ulong.MaxValue, ulong.MinValue }, encoded);
            Assert.Equal(4, VarintEncoder.GetIntegerCount(data.AsSpan()));
        }

        [Fact]
        public void FindFirstTest()
        {
            var data = new byte[30];
            int pos = 0;
            pos += VarintEncoder.Encode(1023, data.AsSpan(pos));
            pos += VarintEncoder.Encode(5, data.AsSpan(pos));
            pos += VarintEncoder.Encode(ulong.MaxValue, data.AsSpan(pos));
            pos += VarintEncoder.Encode(ulong.MinValue, data.AsSpan(pos));

            int posFirst = VarintEncoder.FindFirst(data.AsSpan());
            ulong first = VarintEncoder.Decode(data.AsSpan(0, posFirst));

            Assert.Equal(1023ul, first);
        }

        [Fact]
        public void FindLastTest()
        {
            var data = new byte[30];
            int pos = 0;
            pos += VarintEncoder.Encode(1023, data.AsSpan(pos));
            pos += VarintEncoder.Encode(5, data.AsSpan(pos));
            pos += VarintEncoder.Encode(ulong.MaxValue, data.AsSpan(pos));
            pos += VarintEncoder.Encode(ulong.MinValue, data.AsSpan(pos));
            pos += VarintEncoder.Encode(719, data.AsSpan(pos));

            int posLast = VarintEncoder.FindLast(data.AsSpan());
            ulong last = VarintEncoder.Decode(data.AsSpan(posLast));

            Assert.Equal(719ul, last);
        }
    }
}
