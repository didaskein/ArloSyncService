using ArloSyncService.Common.Extensions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArloSyncService.Converter
{
    public class UnixSecondsDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTimeExtensions.FromUnixTimeSeconds(reader.GetInt64());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ConvertToUnixTimeSeconds());
        }
    }
}
