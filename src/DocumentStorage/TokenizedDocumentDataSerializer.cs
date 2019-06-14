using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentStorage
{
    public class TokenizedDocumentDataSerializer : IDocumentDataSerializer<IEnumerable<string>>
    {
        private static IDocumentDataSerializer<string> stringSerializer = new StringDocumentDataSerializer();

        public IEnumerable<string> Deserialize(Stream stream)
        {
            return stringSerializer.Deserialize(stream).Split().Where(s => s != string.Empty);
        }

        public void Serialize(Stream stream, IEnumerable<string> data)
        {
            stringSerializer.Serialize(stream, string.Join(' ', data));
        }

        public string FileExtension => ".txt";
    }
}
