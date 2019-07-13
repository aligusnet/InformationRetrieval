using Corpus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InformationRetrieval.Indexing
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
            foreach (var id in postings)
            {
                writer.Write(id.Id);
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
    }
}
