using Corpus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InformationRetrieval.Indexing
{
    public class PostingsListReader : IDisposable
    {
        private readonly Stream stream;
        private readonly BinaryReader reader;

        public PostingsListReader(Stream stream, bool leaveOpen)
        {
            this.stream = stream;
            reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen);
        }

        public IReadOnlyCollection<DocumentId> Read(long position)
        {
            stream.Seek(position, SeekOrigin.Begin);
            var length = reader.ReadInt32();
            var postings = new DocumentId[length];
            for (int i = 0; i < length; ++i)
            {
                postings[i] = new DocumentId(reader.ReadUInt32());
            }
            return postings;
        }

        public int ReadCount(long position)
        {
            stream.Seek(position, SeekOrigin.Begin);
            return reader.ReadInt32();
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
