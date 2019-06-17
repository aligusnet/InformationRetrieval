using Xunit;
using DawgSharp;

using NaturalLanguageTools.Indexing;

namespace NaturalLanguageToolsUnitTests.Indexing
{
    public class DawgBuildableIndexUnitTests : IndexUnitTestsBase<DawgSearchableIndex>
    {
        [Fact]
        public void TryGetValueUnexpectedBehaviour()
        {
            var builder = new DawgBuilder<string>();
            builder.Insert("dates", "dates");

            bool b = builder.TryGetValue("date", out var v);
            Assert.True(b);
            Assert.Null(v);
        }

        protected override DawgSearchableIndex CreateIndex(string[][] storage)
        {
            var buildableIndex = new DawgBuildableIndex();
            BuildIndex(buildableIndex, storage);
            return buildableIndex.CreateIndex();

        }
    }
}
