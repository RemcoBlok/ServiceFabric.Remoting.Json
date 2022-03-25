using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceFabric.Remoting.Json
{
    public class DateOnlyNullableConverter : JsonConverter<DateOnly?>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == null)
            {
                return null;
            }

            try
            {
                return DateOnly.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }
}
