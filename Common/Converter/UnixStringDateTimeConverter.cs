using ArloSyncService.Common.Extensions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArloSyncService.Converter
{
    public class UnixStringDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? strdate = reader.GetString();
            if (long.TryParse(strdate, out long ldate))
            {
                return DateTimeExtensions.FromUnixTimeSeconds(ldate);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ConvertToUnixTimeSeconds());
        }
    }
}
