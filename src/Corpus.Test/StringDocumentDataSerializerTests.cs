using Xunit;

using System.IO;

namespace Corpus.Test
{
    public class StringDocumentDataSerializerTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("Simple string")]
        public void StringSerializeAndDeserializeTest(string data)
        {

            var stream = new MemoryStream();
            var serializer = new StringDocumentDataSerializer();
            serializer.Serialize(stream, data);

            stream.Seek(0, SeekOrigin.Begin);

            var deserializedData = serializer.Deserialize(stream);

            Assert.Equal(data, deserializedData);
        }
    }
}
