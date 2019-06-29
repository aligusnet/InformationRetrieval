using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using NaturalLanguageTools.Utility;

namespace NaturalLanguageToolsUnitTests.Utility
{
    public class TemporaryBufferUnitTests
    {
        [Fact]
        public void TemporaryBuffer_Add()
        {
            using var buffer = new TemporaryBuffer<int>(1);

            Assert.Equal(0, buffer.Count);

            buffer.Add(100);
            Assert.Equal(1, buffer.Count);
            Assert.True(buffer.EndsWith(100));
            Assert.False(buffer.EndsWith(100, 200));
            Assert.False(buffer.EndsWith(100, 200, 300));
            Assert.Equal(100, buffer.Last());

            buffer.Add(200);
            Assert.Equal(2, buffer.Count);
            Assert.False(buffer.EndsWith(100));
            Assert.True(buffer.EndsWith(100, 200));
            Assert.False(buffer.EndsWith(100, 200, 300));
            Assert.Equal(200, buffer.Last());

            buffer.DiscardLast();
            Assert.Equal(1, buffer.Count);
            Assert.True(buffer.EndsWith(100));
            Assert.False(buffer.EndsWith(100, 200));
            Assert.False(buffer.EndsWith(100, 200, 300));
            Assert.Equal(100, buffer.Last());

            buffer.Add(200);
            buffer.Add(300);
            Assert.Equal(3, buffer.Count);
            Assert.False(buffer.EndsWith(100));
            Assert.True(buffer.EndsWith(200, 300));
            Assert.True(buffer.EndsWith(100, 200, 300));
            Assert.Equal(300, buffer.Last());
        }

        [Fact]
        public void TemporaryBuffer_Apply()
        {
            using var buffer = new TemporaryBuffer<int>(1)
            {
                100,
                200,
                300
            };

            Func<int[], int, int> sumf = (data, pos) => data.AsSpan().Slice(0, pos).ToArray().Sum();

            int sum = buffer.Apply(sumf);
            Assert.Equal(600, sum);

            sum = 0;
            buffer.Apply((data, pos) => sum = sumf(data, pos));
            Assert.Equal(600, sum);
        }

        [Fact]
        public void TemporaryBuffer_DisposeCanBeCalledManyTimes()
        {
            using var buffer = new TemporaryBuffer<int>(1)
            {
                100,
                200,
                300
            };

            buffer.Dispose();
            buffer.Dispose();
        }

        [Fact]
        public void TemporaryBuffer_Insert()
        {
            using var buffer = new TemporaryBuffer<int>(1)
            {
                100,
                200,
                300
            };

            buffer.Insert(2, 700);

            Assert.True(buffer.EndsWith(700, 300));
        }

        [Fact]
        public void TemporaryBuffer_DiscardLast()
        {
            using var buffer = new TemporaryBuffer<int>(1)
            {
                100,
                200,
                300
            };

            buffer.DiscardLast();

            Assert.Equal(new[] { 100, 200 }, buffer.ToArray());
        }

        [Fact]
        public void TemporaryBuffer_Discard()
        {
            using var buffer = new TemporaryBuffer<int>(1)
            {
                100,
                200,
                300
            };

            buffer.Discard();

            Assert.Equal(0, buffer.Count);
            Assert.Empty(buffer);
        }

        [Fact]
        public void TemporaryBuffer_Commit()
        {
            using var buffer = new TemporaryBuffer<int>(1)
            {
                100,
                200,
                300
            };

            buffer.DiscardLast();
            buffer.Add(700);
            buffer.Add(900);

            var data = new List<int>();
            buffer.Commit(data);

            Assert.Equal(new List<int> { 100, 200, 700, 900}, data);
        }

        [Fact]
        public void TemporaryBuffer_CommitAfterDicard()
        {
            using var buffer = new TemporaryBuffer<int>(1)
            {
                100,
                200,
                300
            };

            buffer.DiscardLast();
            buffer.Add(700);
            buffer.Add(900);
            buffer.Discard();

            var data = new List<int>();
            buffer.Commit(data);

            Assert.Empty(data);
        }
    }
}
