﻿using System.ComponentModel;
using System.Runtime.Serialization;

namespace PVRPCloud;

[Serializable]
[KnownType(typeof(List<PVRPCloudCalcTask>))]
[KnownType(typeof(PVRPCloudResErrMsg))]
public class PVRPCloudResult
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

    // [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public PVRPCloudResultStatus Status { get; set; }
    public string ItemID { get; set; } = string.Empty;
    public required object Data { get; set; }
}
