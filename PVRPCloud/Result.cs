using System.ComponentModel;
using System.Runtime.Serialization;
using PVRPCloud.Requests;

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

    public ResultStatus Status { get; private set; }
    public string ItemID { get; private set; } = string.Empty;
    public Project? Project { get; private set; }
    public ProjectRes? ProjectResult { get; private set; }
    public ResErrMsg? ResErrMsg { get; private set; }

    private Result() {}

    public static Result Success(Project project) => new()
    {
        Status = ResultStatus.RESULT,
        Project = project
    };

    public static Result Success(ProjectRes projectResult) => new()
    {
        Status = ResultStatus.RESULT,
        ProjectResult = projectResult,
    };

    public static Result ValidationError(ResErrMsg error, string itemId) => new()
    {
        ItemID = itemId,
        Status = ResultStatus.VALIDATIONERROR,
        ResErrMsg = error,
    };

    public static Result Error(ResErrMsg error, string itemId = "") => new()
    {
        ItemID = itemId,
        Status = ResultStatus.ERROR,
        ResErrMsg = error,
    };

    public static Result Exception(ResErrMsg error) => new()
    {
        Status = ResultStatus.EXCEPTION,
        ResErrMsg = error,
    };
}
