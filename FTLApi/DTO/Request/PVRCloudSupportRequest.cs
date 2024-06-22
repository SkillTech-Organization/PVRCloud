using Newtonsoft.Json;
using PVRPCloud;

namespace PVRCloudApi.DTO.Request;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class PVRCloudSupportRequest
{
    [JsonProperty("maxTruckDistance")]
    public int MaxTruckDistance { get; set; }

    [JsonProperty("taskList")]
    public List<PVRPCloudTask> TaskList { get; set; }

    [JsonProperty("truckList")]
    public List<PVRPCloudTruck> TruckList { get; set; }
}