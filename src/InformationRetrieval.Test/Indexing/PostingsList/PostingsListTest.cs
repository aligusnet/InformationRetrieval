using System.IO;

using Xunit;

using InformationRetrieval.Indexing.PostingsList;
using InformationRetrieval.Utility;
using Corpus;
using System.Linq;

namespace InformationRetrieval.Test.Indexing.PostingsList
{
    public class PostingsListTest
    {
        [Fact]
        public void UncompressedReadWriteTest()
        {
            var stream = new MemoryStream();

            var postings = GetDocIds(0, 1, 2, 10, 11, 12, 13, 14, 15, 100, 111);

            using var writer = new PostingsListWriter(stream);
            writer.Write(postings);

            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new PostingsListReader(stream, leaveOpen: false);
            var count = reader.ReadCount(0);
            var deserialized = reader.Read(0);

            Assert.Equal(postings.Length, count);
            Assert.False(deserialized is RangePostingsList);
            Assert.Equal(postings, deserialized);
        }

        [Fact]
        public void RangeReadWriteTest()
        {
            var stream = new MemoryStream();

            var rangePostings = new RangePostingsList()
            {
                0, 1, 2, 10, 11, 12, 13, 14, 15, 100, 111
            };

            using var writer = new PostingsListWriter(stream);
            writer.Write(rangePostings);

            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new PostingsListReader(stream, leaveOpen: false);
            var count = reader.ReadCount(0);
            var deserialized = reader.Read(0);

            Assert.Equal(rangePostings.Count, count);
            Assert.True(deserialized is RangePostingsList);
            Assert.Equal(rangePostings, deserialized);
        }

        [Fact]
        public void VarintReadWriteTest()
        {
            var stream = new MemoryStream();

            var postings = new VarintPostingsList()
            {
                0, 1, 2, 10, 11, 12, 13, 14, 15, 100, 111
            };

            using var writer = new PostingsListWriter(stream);
            writer.Write(postings);

            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new PostingsListReader(stream, leaveOpen: false);
            var count = reader.ReadCount(0);
            var deserialized = reader.Read(0);

            Assert.Equal(postings.Count, count);
            Assert.True(deserialized is VarintPostingsList);
            Assert.Equal(postings, deserialized);
        }

        [Fact]
        public void ChainedRangeReadWriteTest()
        {
            var stream = new MemoryStream();

            var chain = new ListChain<DocumentId>()
            {
                new RangePostingsList()
                {
                    0, 1, 2
                },
                GetDocIds(10, 11),
                new RangePostingsList()
                {
                    12, 13, 14, 15, 100, 111
                },
            };

            using var writer = new PostingsListWriter(stream);
            writer.Write(chain);

            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new PostingsListReader(stream, leaveOpen: false);
            var count = reader.ReadCount(0);
            var deserialized = reader.Read(0);

            Assert.Equal(chain.Count, count);
            Assert.True(deserialized is RangePostingsList);
            Assert.Equal(chain, deserialized);
        }

        [Fact]
        public void ChainedUncompressedReadWriteTest()
        {
            var stream = new MemoryStream();

            var chain = new ListChain<DocumentId>()
            {
                GetDocIds(0, 1, 2),
                GetDocIds(10, 11),
                new RangePostingsList()
                {
                    12, 13, 14, 15, 100, 111
                },
            };

            using var writer = new PostingsListWriter(stream);
            writer.Write(chain);

            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new PostingsListReader(stream, leaveOpen: false);
            var count = reader.ReadCount(0);
            var deserialized = reader.Read(0);

            Assert.Equal(chain.Count, count);
            Assert.False(deserialized is RangePostingsList);
            Assert.Equal(chain, deserialized);
        }

        public static DocumentId[] GetDocIds(params uint[] ids)
        {
            return ids.Select(id => new DocumentId(id)).ToArray();
        }
    }
}
