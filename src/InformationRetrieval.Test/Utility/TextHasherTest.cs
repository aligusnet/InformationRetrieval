using System;
using System.Linq;

using Xunit;

using InformationRetrieval.Utility;

namespace InformationRetrieval.Test.Utility
{
    public class TextHasherTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("abcd")]
        [InlineData("crazy text @# \" -+==-08")]
        public void HashFunctionsMustBeIdentical(string text)
        {
            var hash1 = TextHasher.CalculateHashCode(text.ToList(), 0, text.Length);
            var hash2 = TextHasher.CalculateHashCode(text.AsSpan());
            Assert.Equal(hash1, hash2);
        }
    }
}
