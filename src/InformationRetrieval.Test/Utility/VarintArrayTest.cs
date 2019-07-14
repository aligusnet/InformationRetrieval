using Xunit;
using InformationRetrieval.Utility;
using System;
using System.Linq;
using System.IO;

namespace InformationRetrieval.Test.Utility
{
    public class VarintArrayTest
    {
        [Fact]
        public void VarintArrayCreateTest()
        {
            var array = new VarintArray()
            {
                1023, 5, ulong.MaxValue, ulong.MinValue
            };

            Assert.Equal(new ulong[] { 1023, 5, ulong.MaxValue, ulong.MinValue }, array);
        }

        [Fact]
        public void VarintArrayReadWriteTest()
        {
            var stream = new MemoryStream();

            var array = new VarintArray()
            {
                15, 199, 100, 17
            };

            stream.Write(array.GetReadOnlySpan());

            var numBytes = array.GetReadOnlySpan().Length;

            stream.Seek(0, SeekOrigin.Begin);

            var buffer = new byte[numBytes];
            stream.Read(buffer.AsSpan());

            var restored = new VarintArray(buffer);

            Assert.Equal(array, restored);
        }
    }
}
