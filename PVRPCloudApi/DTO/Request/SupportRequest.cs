using Newtonsoft.Json;
using PVRPCommon;

namespace PVRPCloudApi.DTO.Request;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class SupportRequest
{
    [JsonProperty("maxTruckDistance")]
    public int MaxTruckDistance { get; set; }

    [JsonProperty("taskList")]
    public List<PVRPTask> TaskList { get; set; }

    [JsonProperty("truckList")]
    public List<Truck> TruckList { get; set; }
}