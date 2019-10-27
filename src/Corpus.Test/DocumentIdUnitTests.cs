using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Corpus.Test
{
    public class DocumentIdUnitTests
    {
        [Theory]
        [InlineData(0, 0, "00000000")]
        [InlineData(1, 2, "00010002")]
        [InlineData(15, 2, "000F0002")]
        [InlineData(1458, 33124, "05B28164")]
        public void DocumentIdFormattingTest(ushort blockId, ushort localId, string formatted)
        {
            var docId = new DocumentId(blockId, localId);
            Assert.Equal(formatted, docId.ToString());
        }

        [Fact]
        public void DocumentIdCanBeKeyInDictionary()
        {
            var docIds = new[]
            {
                new DocumentId(0, 0),
                new DocumentId(11, 15),
                new DocumentId(1000, 738),
            };

            var dic = docIds.ToDictionary(d => d, d => d.ToString());

            foreach (var docId in docIds)
            {
                Assert.Equal(docId.ToString(), dic[docId].ToString());
            }
        }

        [Fact]
        public void DocumentIdEqualityTest()
        {
            var docId11a = new DocumentId(1, 1);
            var docId11b = new DocumentId(1, 1);
            var docId21a = new DocumentId(2, 1);

            Assert.True(docId11a == docId11b);
            Assert.False(docId11a != docId11b);

            Assert.False(docId11a == docId21a);
            Assert.True(docId11a != docId21a);

            Assert.True(docId11a.Equals(docId11b));
            Assert.False(docId11a.Equals(docId21a));

            var hashSet = new HashSet<DocumentId>
            {
                docId11a,
                docId11b,
            };

            Assert.Single(hashSet);
            Assert.Contains(docId11b, hashSet);
            Assert.DoesNotContain(docId21a, hashSet);
        }
    }
}
