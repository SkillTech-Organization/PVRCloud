using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using static PVRPCloud.Result;

namespace PVRPCloudApi.Util;

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

public class EnumConverter : JsonConverter<ResultStatus>
{
    public override ResultStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (ResultStatus)Enum.Parse(typeof(ResultStatus), reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, ResultStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
