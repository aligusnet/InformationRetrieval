using System.IO;

using Xunit;

using InformationRetrieval.Indexing.InMemory;

namespace InformationRetrieval.Test.Indexing.InMemory
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

        protected override DictionaryIndex<string> CreateIndex(string[][] corpus)
        {
            var index = new DictionaryIndex<string>(rareWordThreshold: 3);
            IndexHelper.BuildIndex(index, corpus);
            return index;
        }
    }
}
