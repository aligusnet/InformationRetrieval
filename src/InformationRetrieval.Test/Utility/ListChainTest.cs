using System.Linq;

using Xunit;

using InformationRetrieval.Utility;
using System;

namespace InformationRetrieval.Test.Utility
{
    public class ListChainTest
    {
        [Fact]
        public void ListChainCreateTest()
        {
            var chain = new ListChain<int>
            {
                new[] { 4, 5, 6, 7 },
                new[] { 1, 2 },
                new[] { 3 }
            };

            Assert.Equal(7, chain.Count);
            Assert.Equal(new[] { 4, 5, 6, 7, 1, 2, 3 }, chain.ToArray());
        }

        [Fact]
        public void ListChainSortTest()
        {
            var chain = new ListChain<int>
            {
                new[] { 4, 5, 6, 7 },
                new[] { 1, 2 },
                new[] { 3 }
            };

            chain.Sort((a, b) => a[0].CompareTo(b[0]));

            Assert.Equal(7, chain.Count);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7 }, chain.ToArray());
        }

        [Fact]
        public void ListChainClearTest()
        {
            var chain = new ListChain<int>
            {
                new[] { 4, 5, 6, 7 },
                new[] { 1, 2 },
                new[] { 3 }
            };

            chain.Clear();

            Assert.Empty(chain);
            Assert.Equal(Array.Empty<int>(), chain.ToArray());
        }
    }
}
