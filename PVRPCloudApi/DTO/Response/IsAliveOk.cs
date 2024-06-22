using Newtonsoft.Json;

namespace PVRPCloudApi.DTO.Response;

public class IsAliveOk
{
    [JsonProperty("version")]
    public string Version;
}
