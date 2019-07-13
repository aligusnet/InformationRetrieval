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
            ushort length = reader.ReadUInt16();
            var blocks = new List<DocumentIdRangeBlock>(length);
            for (int i = 0; i < length; ++i)
            {
                blocks.Add(ReadBlock());
            }

            return new RangePostingsList(count, blocks);
        }

        private DocumentIdRangeBlock ReadBlock()
        {
            ushort blockId = reader.ReadUInt16();
            ushort length = reader.ReadUInt16();
            var ranges = new List<uint>(length);
            for (int i = 0; i < length; ++i)
            {
                ranges.Add(reader.ReadUInt32());
            }

            return new DocumentIdRangeBlock(blockId, ranges);
        }
    }
}
