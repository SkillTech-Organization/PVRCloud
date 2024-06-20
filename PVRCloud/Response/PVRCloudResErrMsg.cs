namespace PVRCloud.Response;

[Serializable]
public class FTLResErrMsg
{
    public string Field { get; set; }
    public string Message { get; set; }
    public string CallStack { get; set; }
}
