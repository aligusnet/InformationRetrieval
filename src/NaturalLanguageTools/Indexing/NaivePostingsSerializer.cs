using System.IO;
using System.Text;

namespace NaturalLanguageTools.Indexing
{
    public class NaivePostingsSerializer
    {
        public static void Serialize(Stream destination, uint[] postings)
        {
            using var writer = new BinaryWriter(destination, Encoding.UTF8, leaveOpen: true);
            writer.Write(postings.Length);
            foreach (uint id in postings)
            {
                writer.Write(id);
            }
        }

        public static uint[] Deserialize(Stream source)
        {
            using var reader = new BinaryReader(source, Encoding.UTF8, leaveOpen: true);
            var length = reader.ReadInt32();
            var postings = new uint[length];
            for (int i = 0; i < length; ++i)
            {
                postings[i] = reader.ReadUInt32();
            }
            return postings;
        }

        public static int DeserializeCount(Stream source)
        {
            using var reader = new BinaryReader(source, Encoding.UTF8, leaveOpen: true);
            return reader.ReadInt32();
        }
    }
}
