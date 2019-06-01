using System.IO;

namespace DocumentStorage
{
    public class StringDocumentDataSerializer : IDocumentDataSerializer<string>
    {
        public string Deserialize(Stream stream)
        {
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public void Serialize(Stream stream, string data)
        {
            using var writer = new StreamWriter(stream);

            writer.Write(data);
        }
    }
}
