using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PVRCloud.Response;

[Serializable]
[KnownType(typeof(List<PVRCloudCalcTask>))]
[KnownType(typeof(PVRCloudResErrMsg))]
public class PVRCloudResult
{
    public enum PVRCloudResultStatus
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

    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public PVRCloudResultStatus Status { get; set; }
    public string ObjectName { get; set; }
    public string ItemID { get; set; }
    public object Data { get; set; }

}
