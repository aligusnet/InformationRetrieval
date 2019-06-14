using System.IO;

namespace DocumentStorage
{
    public class StringDocumentDataSerializer : IDocumentDataSerializer<string>
    {
        public string Deserialize(Stream stream)
        {
            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public void Serialize(Stream stream, string data)
        {
            var writer = new StreamWriter(stream);

            writer.Write(data);

            writer.Flush();
        }

        public string FileExtension => ".txt";
    }
}
