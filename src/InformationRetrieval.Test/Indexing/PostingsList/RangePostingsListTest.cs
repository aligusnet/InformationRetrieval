using System;

using Xunit;

using InformationRetrieval.Indexing.PostingsList;
using Corpus;
using System.Linq;

namespace InformationRetrieval.Test.Indexing.PostingsList
{
    public class RangePostingsListTest
    {
        [Fact]
        public void EmptyRangeTest()
        {
            var range = new RangePostingsList();

            Assert.Empty(range);
            Assert.Equal(Array.Empty<DocumentId>(), range.ToArray());
        }

        [Fact]
        public void RangeWithRepeatingDocIdTest()
        {
            var range = new RangePostingsList
            {
                0, 1, 1, 1, 2, 3, 10, 11, 11, 11, 12
            };

            Assert.Equal(7, range.Count);
            Assert.Equal(
                new uint[] { 0, 1, 2, 3, 10, 11, 12 }.Select(u => new DocumentId(u)).ToArray(),
                range.ToArray());
        }
    }
}
