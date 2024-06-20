using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using static FTLSupporter.FTLResult;

namespace PVRCloudApi.Util;

public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture));
    }
}

public class EnumConverter : JsonConverter<FTLResultStatus>
{
    public override FTLResultStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (FTLResultStatus)Enum.Parse(typeof(FTLResultStatus), reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, FTLResultStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
