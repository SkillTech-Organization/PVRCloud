using System.ComponentModel;
using System.Runtime.Serialization;

namespace PVRPCloud;

[Serializable]
[KnownType(typeof(List<CalcTask>))]
[KnownType(typeof(ResErrMsg))]
public sealed class Result
{
    public enum ResultStatus
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

    public ResultStatus Status { get; set; }
    public string ItemID { get; set; } = string.Empty;
    public required object Data { get; set; }

    public static Result Success(object obj) => new()
    {
        Status = ResultStatus.RESULT,
        Data = obj
    };

    public static Result ValidationError(ResErrMsg error, string itemId) => new()
    {
        ItemID = itemId,
        Status = ResultStatus.VALIDATIONERROR,
        Data = error,
    };

    public static Result Exception(ResErrMsg error) => new()
    {
        Status = ResultStatus.EXCEPTION,
        Data = error,
    };
}
