using System.Linq;

using Xunit;

namespace Corpus.Test
{
    public class DocumentIdUnitTests
    {
        [Theory]
        [InlineData(1, 1, 65536 + 1)]
        [InlineData(2, 11, 2*65536 + 11)]
        [InlineData(0, 0, 0)]
        public void DocumentIdDeconstructorTest(ushort blockId, ushort localId, uint id)
        {
            var docId = new DocumentId(blockId, localId);

            Assert.Equal(id, docId.Id);
            Assert.Equal(blockId, docId.BlockId);
            Assert.Equal(localId, docId.LocalId);
        }

        [Theory]
        [InlineData(0, 0, "00000000")]
        [InlineData(1, 2, "00010002")]
        [InlineData(15, 2, "000F0002")]
        [InlineData(1458, 33124, "05B28164")]
        public void DocumentIdFormattingTest(ushort blockId, ushort localId, string formatted)
        {
            var docId = new DocumentId(blockId, localId);
            Assert.Equal(formatted, docId.ToString());
            Assert.Equal(formatted, docId.BlockIdString() + docId.LocalIdString());
        }

        [Theory]
        [InlineData("00000000", 0, 0)]
        [InlineData("00010002", 1, 2)]
        [InlineData("000F0002", 15, 2)]
        [InlineData("05B28164", 1458, 33124)]
        public void ParseDocumentIdTest(string hex, ushort blockId, ushort localId)
        {
            var docId = DocumentId.Parse(hex);
            Assert.Equal(blockId, docId.BlockId);
            Assert.Equal(localId, docId.LocalId);
        }

        [Theory]
        [InlineData("0000", 0)]
        [InlineData("0001", 1)]
        [InlineData("000F", 15)]
        [InlineData("05B2", 1458)]
        public void ParseLocalIdTest(string hex, ushort localId)
        {
            var parsedId = DocumentId.ParseLocalId(hex);
            Assert.Equal(localId, parsedId);
        }

        [Theory]
        [InlineData("0000", 0)]
        [InlineData("0001", 1)]
        [InlineData("000F", 15)]
        [InlineData("05B2", 1458)]
        public void ParseBlockIdTest(string hex, ushort blockId)
        {
            var parsedId = DocumentId.ParseBlockId(hex);
            Assert.Equal(blockId, parsedId);
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
    }
}
