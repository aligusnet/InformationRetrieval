using System.IO;

using Xunit;

using NaturalLanguageTools.Indexing;

namespace NaturalLanguageToolsUnitTests.Indexing
{
    public class DictionaryIndexUnitTests : IndexUnitTestsBase<DictionaryIndex<string>>
    {
        [Fact]
        public void SerializationTest()
        {
            using var stream = new MemoryStream();

            var index = CreateIndex();
            index.Serialize(stream);

            stream.Seek(0, SeekOrigin.Begin);
            var deserializedIndex = DictionaryIndex<string>.Deserialize(stream);

            AssertIndices(index, deserializedIndex);
        }

        protected override DictionaryIndex<string> CreateIndex(string[][] storage)
        {
            var index = new DictionaryIndex<string>();
            BuildIndex(index, storage);
            return index;
        }
    }
}
