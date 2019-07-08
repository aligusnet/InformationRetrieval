using Xunit;
using DawgSharp;

using InformationRetrieval.Indexing;
using System.IO;

namespace InformationRetrieval.Test.Indexing
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

        [Fact]
        public void SerializationTest()
        {
            using var stream = new MemoryStream();

            var index = CreateIndex();
            index.Serialize(stream);

            stream.Seek(0, SeekOrigin.Begin);
            var deserializedIndex = DawgSearchableIndex.Deserialize(stream);

            AssertIndices(index, deserializedIndex);
        }

        protected override DawgSearchableIndex CreateIndex(string[][] corpus)
        {
            var buildableIndex = new DawgBuildableIndex();
            IndexHelper.BuildIndex(buildableIndex, corpus);
            return buildableIndex.Build();
        }
    }
}
