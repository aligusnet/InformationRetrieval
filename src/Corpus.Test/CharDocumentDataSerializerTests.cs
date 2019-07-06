using Xunit;

using System.IO;

namespace Corpus.Test
{
    public class CharDocumentDataSerializerTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("Simple string")]
        public void StringSerializeAndDeserializeTest(string str)
        {
            var data = str.ToCharArray();
            var stream = new MemoryStream();
            var serializer = new CharDocumentDataSerializer();
            serializer.Serialize(stream, data);

            stream.Seek(0, SeekOrigin.Begin);

            var deserializedData = serializer.Deserialize(stream);

            Assert.Equal(data, deserializedData);
        }
    }
}
