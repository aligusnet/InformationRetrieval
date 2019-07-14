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
    }
}
