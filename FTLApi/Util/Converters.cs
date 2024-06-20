using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using static PVRCloud.PVRCloudResult;

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

public class EnumConverter : JsonConverter<PVRCloudResultStatus>
{
    public override PVRCloudResultStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (PVRCloudResultStatus)Enum.Parse(typeof(PVRCloudResultStatus), reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, PVRCloudResultStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
