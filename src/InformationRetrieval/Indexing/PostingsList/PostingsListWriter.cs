using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Corpus;

namespace InformationRetrieval.Indexing.PostingsList
{
    public class PostingsListWriter : IDisposable
    {
        private readonly Stream stream;
        private readonly BinaryWriter writer;

        public PostingsListWriter(Stream stream)
        {
            this.stream = stream;
            writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        }

        public long Write(IReadOnlyCollection<DocumentId> postings)
        {
            var position = stream.Position;
            writer.Write(postings.Count);

            switch (postings)
            {
                case RangePostingsList range:
                    writer.Write((byte)PostingsListType.Ranged);
                    WriteRanged(range.Blocks);
                    break;

                default:
                    writer.Write((byte)PostingsListType.Uncompressed);
                    WriteUncompressed(postings);
                    break;
            }
            
            writer.Flush();

            return position;
        }

        public Stream Reset()
        {
            writer.Dispose();

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public void Dispose()
        {
            writer.Dispose();
        }

        private void WriteUncompressed(IReadOnlyCollection<DocumentId> postings)
        {
            foreach (var id in postings)
            {
                writer.Write(id.Id);
            }
        }

        private void WriteRanged(IList<DocumentIdRangeBlock> list)
        {
            writer.Write((ushort)list.Count);
            foreach (var block in list)
            {
                WriteBlock(block);
            }
        }

        private void WriteBlock(DocumentIdRangeBlock block)
        {
            writer.Write(block.BlockId);
            writer.Write((ushort)block.Ranges.Count);
            foreach (uint val in block.Ranges)
            {
                writer.Write(val);
            }
        }
    }
}
