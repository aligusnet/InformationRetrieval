using System;
using System.Buffers;
using System.IO;
using System.Text.Json;

namespace Corpus.Json
{
    public ref struct Utf8JsonStreamReader
    {
        private readonly Stream stream;
        private IMemoryOwner<byte> buffer;

        private Utf8JsonReader jsonReader;
        private int dataSize;

        public Utf8JsonStreamReader(Stream stream)
        {
            this.stream = stream;
            buffer = MemoryPool<byte>.Shared.Rent(32 * 1024);

            jsonReader = default;
            dataSize = -1;
        }

        public void Dispose()
        {
            buffer.Dispose();
        }

        public bool Read()
        {
            // read could be unsuccessful due to insufficient buffer size, retrying in loop with increasing buffer sizes
            while (!jsonReader.Read())
            {
                if (dataSize == 0)
                    return false;

                MoveNext();
            }

            return true;
        }

        private void MoveNext()
        {
            int leftOver = 0;

            if (dataSize != -1)
            {
                leftOver = dataSize - (int)jsonReader.CurrentState.BytesConsumed;

                if (leftOver == buffer.Memory.Length)
                {
                    // current JSON token is to large to fit in buffer, try growing buffer
                    var newBuffer = MemoryPool<byte>.Shared.Rent(2 * buffer.Memory.Length);

                    buffer.Memory.CopyTo(newBuffer.Memory);
                    buffer.Dispose();

                    buffer = newBuffer;
                }

                if (leftOver != 0)
                {
                    // we haven't read to the end of previous buffer, carry forward
                    buffer.Memory.Slice(dataSize - leftOver, leftOver).CopyTo(buffer.Memory);
                }
            }

            dataSize = leftOver + stream.Read(buffer.Memory[leftOver..].Span);
            jsonReader = new Utf8JsonReader(buffer.Memory[0..dataSize].Span, dataSize == 0, jsonReader.CurrentState);
        }

        public JsonTokenType TokenType => jsonReader.TokenType;
        public SequencePosition Position => jsonReader.Position;
        public bool HasValueSequence => jsonReader.HasValueSequence;
        public int CurrentDepth => jsonReader.CurrentDepth;
        public long BytesConsumed => jsonReader.BytesConsumed;
        public ReadOnlySequence<byte> ValueSequence => jsonReader.ValueSequence;
        public ReadOnlySpan<byte> ValueSpan => jsonReader.ValueSpan;
        public bool GetBoolean() => jsonReader.GetBoolean();
        public decimal GetDecimal() => jsonReader.GetDecimal();
        public double GetDouble() => jsonReader.GetDouble();
        public int GetInt32() => jsonReader.GetInt32();
        public long GetInt64() => jsonReader.GetInt64();
        public float GetSingle() => jsonReader.GetSingle();
        public string GetString() => jsonReader.GetString();
        public uint GetUInt32() => jsonReader.GetUInt32();
        public ulong GetUInt64() => jsonReader.GetUInt64();
        public bool TryGetDecimal(out decimal value) => jsonReader.TryGetDecimal(out value);
        public bool TryGetDouble(out double value) => jsonReader.TryGetDouble(out value);
        public bool TryGetInt32(out int value) => jsonReader.TryGetInt32(out value);
        public bool TryGetInt64(out long value) => jsonReader.TryGetInt64(out value);
        public bool TryGetSingle(out float value) => jsonReader.TryGetSingle(out value);
        public bool TryGetUInt32(out uint value) => jsonReader.TryGetUInt32(out value);
        public bool TryGetUInt64(out ulong value) => jsonReader.TryGetUInt64(out value);
    }
}
