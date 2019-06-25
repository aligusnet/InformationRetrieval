using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentStorage
{
    public class CharDocumentDataSerializer : IDocumentDataSerializer<IList<char>>
    {
        private readonly Encoding encoding = Encoding.UTF8;

        public string FileExtension => ".txt";

        public IList<char> Deserialize(Stream stream)
        {
            using var reader = new StreamReader(stream, encoding, leaveOpen: true);
            var buffer = ArrayPool<char>.Shared.Rent(1024);

            try
            {
                var result = new List<char>(1024);
                int length = 0;
                while ((length = reader.Read(buffer, 0, buffer.Length)) >0)
                {
                    for (int i = 0; i < length; ++i)
                    {
                        result.Add(buffer[i]);
                    }
                }

                return result;
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }

        public void Serialize(Stream stream, IList<char> data)
        {
            var bufferedStream = new BufferedStream(stream);
            using var writer = new BinaryWriter(bufferedStream, encoding, leaveOpen: true);

            foreach (char ch in data)
            {
                writer.Write(ch);
            }
        }
    }
}
