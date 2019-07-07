using System.Collections.Generic;
using System.IO;
using System.Text;

using Corpus;

namespace NaturalLanguageTools.Indexing
{
    public class NaivePostingsSerializer
    {
        public static void Serialize(Stream destination, IReadOnlyCollection<DocumentId> postings)
        {
            using var writer = new BinaryWriter(new BufferedStream(destination), Encoding.UTF8, leaveOpen: true);
            writer.Write(postings.Count);
            foreach (var id in postings)
            {
                writer.Write(id.Id);
            }
        }

        public static DocumentId[] Deserialize(Stream source)
        {
            using var reader = new BinaryReader(new BufferedStream(source), Encoding.UTF8, leaveOpen: true);
            var length = reader.ReadInt32();
            var postings = new DocumentId[length];
            for (int i = 0; i < length; ++i)
            {
                postings[i] = new DocumentId(reader.ReadUInt32());
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
