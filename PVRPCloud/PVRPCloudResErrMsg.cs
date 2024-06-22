namespace PVRPCloud;

[Serializable]
public class PVRPCloudResErrMsg
{
    public string Field { get; set; }
    public string Message { get; set; }
    public string CallStack { get; set; }
}
