using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NewCrmCore.Infrastructure.Converter
{
    public class BooleanConverter : JsonConverter<Boolean>
    {
        public BooleanConverter()
        {
        }

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var booleanValue = reader.GetString();
                if (Boolean.TryParse(booleanValue, out Boolean value))
                {
                    return value;
                }
            }

            throw new System.Text.Json.JsonException();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}