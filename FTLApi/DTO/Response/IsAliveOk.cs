using Newtonsoft.Json;

namespace PVRCloudApi.DTO.Response;

public class IsAliveOk
{
    [JsonProperty("version")]
    public string Version;
}
