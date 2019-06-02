using System.IO;
using Xunit;

using DocumentStorage;


namespace DocumentStorageUnitTests
{
    public class NumberedDocumentDataSerializerTests
    {
        [Fact]
        public void NumberedSerializeAndDeserializeTest()
        {
            var data = new int[] { 11, 17, 23, 8 };

            var stream = new MemoryStream();
            var serializer = new NumberedDocumentDataSerializer();
            serializer.Serialize(stream, data);

            stream.Seek(0, SeekOrigin.Begin);

            var deserializedData = serializer.Deserialize(stream);

            Assert.Equal(data, deserializedData);
        }
    }
}
