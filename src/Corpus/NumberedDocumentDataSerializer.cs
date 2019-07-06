using System.IO;

namespace Corpus
{
    public class NumberedDocumentDataSerializer : IDocumentDataSerializer<int[]>
    {
        public int[] Deserialize(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var size = reader.ReadInt32();
            var data = new int[size];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = reader.ReadInt32();
            }

            return data;
        }

        public void Serialize(Stream stream, int[] data)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                writer.Write(data[i]);
            }

            writer.Flush();
        }

        public string FileExtension => "";
    }
}
