using Newtonsoft.Json;
using PVRPCloud;

namespace PVRPCloudApi.DTO.Request;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class PVRPCloudSupportRequest
{
    [JsonProperty("maxTruckDistance")]
    public int MaxTruckDistance { get; set; }

    [JsonProperty("taskList")]
    public List<PVRPCloudTask> TaskList { get; set; }

    [JsonProperty("truckList")]
    public List<Truck> TruckList { get; set; }
}