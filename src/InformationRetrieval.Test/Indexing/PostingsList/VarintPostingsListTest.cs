using System;
using System.IO;
using System.Linq;

using Xunit;

using Corpus;
using InformationRetrieval.Indexing.PostingsList;


namespace InformationRetrieval.Test.Indexing.PostingsList
{
    public class VarintPostingsListTest
    {
        [Fact]
        public void CreationTest()
        {
            var postingsList = new VarintPostingsList()
            {
                1, 2, 11, 15, 119
            };

            Assert.Equal(PostingsListTest.GetDocIds(1, 2, 11, 15, 119), postingsList.ToArray());
        }

        [Fact]
        public void ReadWriteTest()
        {
            var postingsList = new VarintPostingsList()
            {
                1, 2, 11, 15, 119
            };

            int size = postingsList.GetReadOnlySpan().Length;
            var stream = new MemoryStream();
            stream.Write(postingsList.GetReadOnlySpan());
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[size];
            stream.Read(buffer.AsSpan());
            var restored = new VarintPostingsList(buffer);

            Assert.Equal(postingsList, restored);
        }
    }
}
