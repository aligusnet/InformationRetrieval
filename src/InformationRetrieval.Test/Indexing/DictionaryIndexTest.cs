using System.IO;

using Xunit;

using InformationRetrieval.Indexing;

namespace InformationRetrieval.Test.Indexing
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
            => IndexHelper.CreateDictionaryIndex(corpus);
    }
}
