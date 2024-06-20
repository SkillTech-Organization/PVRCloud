using PVRCloud;
using Newtonsoft.Json;

namespace PVRCloudApi.DTO.Request;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class PVRCloudSupportRequest
{
    [JsonProperty("maxTruckDistance")]
    public int MaxTruckDistance { get; set; }

    [JsonProperty("taskList")]
    public List<PVRCloudTask> TaskList { get; set; }

    [JsonProperty("truckList")]
    public List<PVRCloudTruck> TruckList { get; set; }
}