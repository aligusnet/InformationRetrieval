using System.IO;
using System.Text;

namespace DocumentStorage
{
    public class StringDocumentDataSerializer : IDocumentDataSerializer<string>
    {
        public string Deserialize(Stream stream)
        {
            var reader = new StreamReader(stream, Encoding.UTF8);

            return reader.ReadToEnd();
        }

        public void Serialize(Stream stream, string data)
        {
            var writer = new StreamWriter(stream, Encoding.UTF8);

            writer.Write(data);

            writer.Flush();
        }

        public string FileExtension => ".txt";
    }
}
