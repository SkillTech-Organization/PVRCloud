
using Newtonsoft.Json;

namespace FTLApi.DTO.Response
{
    public class IsAliveOk
    {
        [JsonProperty("version")]
        public string Version;
    }
}
