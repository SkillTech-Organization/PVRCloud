using System.ComponentModel;
using System.Runtime.Serialization;

namespace PVRPCloud;

[Serializable]
[KnownType(typeof(List<CalcTask>))]
[KnownType(typeof(ResErrMsg))]
public sealed class Result
{
    public enum PVRPCloudResultStatus
    {
        [Description("RESULT")]
        RESULT,
        [Description("VALIDATIONERROR")]
        VALIDATIONERROR,
        [Description("EXCEPTION")]
        EXCEPTION,
        [Description("ERROR")]
        ERROR,
        [Description("LOG")]
        LOG
    };

    public PVRPCloudResultStatus Status { get; set; }
    public string ItemID { get; set; } = string.Empty;
    public required object Data { get; set; }

    public static Result Success(object obj) => new()
    {
        Status = PVRPCloudResultStatus.RESULT,
        Data = obj
    };

    public static Result ValidationError(ResErrMsg error, string itemId) => new()
    {
        ItemID = itemId,
        Status = PVRPCloudResultStatus.VALIDATIONERROR,
        Data = error,
    };

    public static Result Exception(ResErrMsg error) => new()
    {
        Status = PVRPCloudResultStatus.EXCEPTION,
        Data = error,
    };
}
