using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NewCrmCore.Infrastructure.Converter
{
    public class Int32Converter : JsonConverter<Int32>
    {
        public Int32Converter()
        {
        }

        public override Int32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                String stringValue = reader.GetString();
                if (Int32.TryParse(stringValue, out Int32 value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            throw new System.Text.Json.JsonException();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}