using System;
using System.Collections.Generic;
using Xunit;

using DocumentStorage;
using System.IO;

namespace DocumentStorageUnitTests
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
