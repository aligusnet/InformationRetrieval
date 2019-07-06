using Xunit;

using System.IO;
using System.Collections.Generic;

namespace Corpus.Test
{
    public class TokenizedDocumentDataSerializerTests
    {
        [Fact]
        public void StringSerializeAndDeserializeTest()
        {
            var data = new List<string> { "one", "two" };

            var stream = new MemoryStream();
            var serializer = new TokenizedDocumentDataSerializer();
            serializer.Serialize(stream, data);

            stream.Seek(0, SeekOrigin.Begin);

            var deserializedData = serializer.Deserialize(stream);

            Assert.Equal(data, deserializedData);
        }
    }
}
