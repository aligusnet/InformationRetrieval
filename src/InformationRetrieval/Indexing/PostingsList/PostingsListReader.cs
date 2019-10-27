using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Corpus;

namespace InformationRetrieval.Indexing.PostingsList
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
            PostingsListType type = (PostingsListType)reader.ReadByte();

            switch (type)
            {
                case PostingsListType.Ranged:
                    return ReadRanged(length);

                case PostingsListType.Varint:
                    return ReadVarint(length);

                default:
                    return ReadUmcompressed(length);
            }
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

        private VarintPostingsList ReadVarint(int count)
        {
            int length = reader.ReadInt32();
            var buffer = new byte[length];

            int readBytes = reader.Read(buffer, 0, length);
            if (readBytes != length)
            {
                throw new Exception("Failed to read VarintPostingsList");
            }

            return new VarintPostingsList(buffer);
        }

        private IReadOnlyCollection<DocumentId> ReadUmcompressed(int length)
        {
            var postings = new DocumentId[length];
            for (int i = 0; i < length; ++i)
            {
                postings[i] = new DocumentId(reader.ReadUInt32());
            }

            return postings;
        }

        private RangePostingsList ReadRanged(int count)
        {
            int length = reader.ReadInt32();
            var ranges = new List<uint>(length);
            for (int i = 0; i < length; ++i)
            {
                ranges.Add(reader.ReadUInt32());
            }

            return new RangePostingsList(count, ranges);
        }
    }
}
