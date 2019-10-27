using System.IO;
using System.Text.Json;

namespace InformationRetrieval.Utility
{
    public class JsonReaderHelper
    {
        public static void Read(ref Utf8JsonStreamReader reader)
        {
            if (!reader.Read())
            {
                throw new InvalidDataException("Unexpected end of stream");
            }
        }

        public static void ReadToken(ref Utf8JsonStreamReader reader, JsonTokenType token)
        {
            Read(ref reader);

            if (reader.TokenType != token)
            {
                throw new InvalidDataException($"Unexpected token at position {reader.Position}: expected {token} but got {reader.TokenType}");
            }
        }

        public static ushort ReadUInt16(ref Utf8JsonStreamReader reader)
        {
            ReadToken(ref reader, JsonTokenType.Number);
            return (ushort)reader.GetUInt32();
        }

        public static uint ReadUInt32(ref Utf8JsonStreamReader reader)
        {
            ReadToken(ref reader, JsonTokenType.Number);
            return reader.GetUInt32();
        }

        public static string ReadString(ref Utf8JsonStreamReader reader)
        {
            ReadToken(ref reader, JsonTokenType.String);
            return reader.GetString();
        }
    }
}
