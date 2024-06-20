﻿using FTLSupporter;
using Newtonsoft.Json;

namespace PVRCloudApi.DTO.Request;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class PVRCloudSupportRequest
{
    [JsonProperty("maxTruckDistance")]
    public int MaxTruckDistance { get; set; }

    [JsonProperty("taskList")]
    public List<FTLTask> TaskList { get; set; }

    [JsonProperty("truckList")]
    public List<FTLTruck> TruckList { get; set; }
}