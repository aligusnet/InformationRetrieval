using System.Collections.Generic;
using System.Linq;

using Xunit;

using DocumentStorage;


namespace DocumentStorageUnitTests
{
    public class DocumentIdUnitTests
    {
        [Theory]
        [InlineData(1, 1, 65536 + 1)]
        [InlineData(2, 11, 2*65536 + 11)]
        [InlineData(0, 0, 0)]
        public void DocumentIdDeconstructorTest(ushort collectionId, ushort localId, uint id)
        {
            var docId = new DocumentId(collectionId, localId);

            Assert.Equal(id, docId.Id);
            Assert.Equal(collectionId, docId.CollectionId);
            Assert.Equal(localId, docId.LocalId);
        }

        [Theory]
        [InlineData(0, 0, "00000000")]
        [InlineData(1, 2, "00010002")]
        [InlineData(15, 2, "000F0002")]
        [InlineData(1458, 33124, "05B28164")]
        public void DocumentIdFormattingTest(ushort collectionId, ushort localId, string formatted)
        {
            var docId = new DocumentId(collectionId, localId);
            Assert.Equal(formatted, docId.ToString());
            Assert.Equal(formatted, docId.CollectionIdString() + docId.LocalIdString());
        }

        [Theory]
        [InlineData("00000000", 0, 0)]
        [InlineData("00010002", 1, 2)]
        [InlineData("000F0002", 15, 2)]
        [InlineData("05B28164", 1458, 33124)]
        public void ParseDocumentIdTest(string hex, ushort collectionId, ushort localId)
        {
            var docId = DocumentId.Parse(hex);
            Assert.Equal(collectionId, docId.CollectionId);
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
        public void ParseCollectionIdTest(string hex, ushort collectionId)
        {
            var parsedId = DocumentId.ParseCollectionId(hex);
            Assert.Equal(collectionId, parsedId);
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
