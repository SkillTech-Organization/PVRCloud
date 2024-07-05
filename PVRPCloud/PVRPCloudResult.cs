﻿using System.ComponentModel;
using System.Runtime.Serialization;

namespace PVRPCloud;

[Serializable]
[KnownType(typeof(List<PVRPCloudCalcTask>))]
[KnownType(typeof(PVRPCloudResErrMsg))]
public sealed class PVRPCloudResult
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

    public static PVRPCloudResult Success(object obj) => new()
    {
        Status = PVRPCloudResultStatus.RESULT,
        Data = obj
    };

    public static PVRPCloudResult ValidationError(PVRPCloudResErrMsg error) => new()
    {
        Status = PVRPCloudResultStatus.VALIDATIONERROR,
        Data = error,
    };

    public static PVRPCloudResult Exception(PVRPCloudResErrMsg error) => new()
    {
        Status = PVRPCloudResultStatus.EXCEPTION,
        Data = error,
    };
}
