using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PVRPCloud.Requests;

namespace PVRPCloudApiTests;

public class OptimizeRequestTests(WebApplicationFactory<Program> _factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static PVRPCloudProject CreateValidProject()
    {
        return new()
        {
            ProjectName = "Test1",
            MinTime = 0,
            MaxTime = 1440,
            MaxTourDuration = 1440,
            DistanceLimit = 1_000_000,
            CostProfiles = [
                new()
                {
                    ID = "cost1",
                    FixCost = 10_000,
                    HourCost = 1000,
                    KmCost = 100,
                },
                new()
                {
                    ID = "cost2",
                    FixCost = 20_000,
                    HourCost = 2000,
                    KmCost = 200,
                },
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "2T",
                    Capacity1 = 2000,
                    Capacity2 = 0
                },
                new()
                {
                    ID = "7.5T",
                    Capacity1 = 7500,
                    Capacity2 = 0
                },
                new()
                {
                    ID = "12T",
                    Capacity1 = 7500,
                    Capacity2 = 0
                },
            ],
            TruckTypes = [
                new()
                {
                    ID = "TType1",
                    TruckTypeName = "TruckType1 AllZones",
                    RestrictedZones = ["KP1", "ÉP1", "DB1", "HB1", "DP3", "DP1", "CS12", "ÉB1", "ÉB7", "CS7", "DP7", "KV3", "P75", "B35", "P35"],
                    Weight = 2000,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 100,
                        [2] = 70,
                        [3] = 60,
                        [4] = 50,
                        [5] = 40,
                        [6] = 15,
                        [7] = 15,
                    }
                },
                new()
                {
                    ID = "TType2",
                    TruckTypeName = "TruckType2 AllZones",
                    RestrictedZones = ["KP1", "ÉP1", "DB1", "HB1", "DP3", "DP1", "CS12", "ÉB1", "ÉB7", "CS7", "DP7", "KV3", "P75", "B35", "P35"],
                    Weight = 75_000,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70,
                        [2] = 60,
                        [3] = 50,
                        [4] = 40,
                        [5] = 40,
                        [6] = 15,
                        [7] = 15,
                    }
                },
                new()
                {
                    ID = "TType3",
                    TruckTypeName = "TruckType2 NoZone",
                    RestrictedZones = [],
                    Weight = 75_000,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70,
                        [2] = 60,
                        [3] = 50,
                        [4] = 40,
                        [5] = 40,
                        [6] = 15,
                        [7] = 15,
                    }
                }
            ],
            Trucks = [
                new()
                {
                    ID = "TRK1",
                    TruckTypeID = "TType1",
                    TruckName = "TRK-001",
                    ArrDepotMaxTime = 0,
                    CapacityProfileID = "2T",
                    MaxWorkTime = 1440,
                    EarliestStart = 0,
                    LatestStart = 1439,
                },
                new()
                {
                    ID = "TRK2",
                    TruckTypeID = "TType2",
                    TruckName = "TRK-002",
                    ArrDepotMaxTime = 0,
                    CapacityProfileID = "7.5T",
                    MaxWorkTime = 1440,
                    EarliestStart = 0,
                    LatestStart = 1439,
                },
                new()
                {
                    ID = "TRK3",
                    TruckTypeID = "TType3",
                    TruckName = "TRK-003",
                    ArrDepotMaxTime = 0,
                    CapacityProfileID = "12T",
                    MaxWorkTime = 1440,
                    EarliestStart = 0,
                    LatestStart = 1439,
                },
            ],
            Depot = new()
            {
                ID = "WH1",
                DepotName = "Warehouse",
                Lat = 47.623782,
                Lng = 19.120456,
                DepotMinTime = 240,
                DepotMaxTime = 1380,
                ServiceFixTime = 1,
                ServiceVarTime = 0,
            },
            Clients = [
                new()
                {
                    ID = "Cl01",
                    ClientName = "Client01",
                    Lat = 47.504811,
                    Lng = 18.986218,
                    FixService = 10,
                },
                new()
                {
                    ID = "Cl02",
                    ClientName = "Client02",
                    Lat = 47.504811,
                    Lng = 19.722090,
                    FixService = 15,
                },
                new()
                {
                    ID = "Cl03",
                    ClientName = "Client03",
                    Lat = 46.265335,
                    Lng = 20.110305,
                    FixService = 5,
                },
                new()
                {
                    ID = "Cl04",
                    ClientName = "Client04",
                    Lat = 46.265335,
                    Lng = 20.110305,
                    FixService = 5,
                },
            ],
            Orders = [
                new()
                {
                    ID = "ord1",
                    OrderName = "Order1",
                    ClientID = "Cl01",
                    Quantity1 = 1000,
                    Quantity2 = 0,
                    ReadyTime = 0,
                    TruckList = ["TRK1", "TRK2", "TRK3"],
                    OrderServiceTime = 10,
                    OrderMinTime = 480,
                    OrderMaxTime = 960,
                },
                new()
                {
                    ID = "ord2",
                    OrderName = "Order2",
                    ClientID = "Cl01",
                    Quantity1 = 2000,
                    Quantity2 = 0,
                    ReadyTime = 0,
                    TruckList = ["TRK1", "TRK2", "TRK3"],
                    OrderServiceTime = 10,
                    OrderMinTime = 480,
                    OrderMaxTime = 960,
                },
                new()
                {
                    ID = "ord3",
                    OrderName = "Order3",
                    ClientID = "Cl03",
                    Quantity1 = 3000,
                    Quantity2 = 0,
                    ReadyTime = 0,
                    TruckList = ["TRK1", "TRK2", "TRK3"],
                    OrderServiceTime = 10,
                    OrderMinTime = 480,
                    OrderMaxTime = 960,
                },
                new()
                {
                    ID = "ord4",
                    OrderName = "Order4",
                    ClientID = "Cl03",
                    Quantity1 = 1,
                    Quantity2 = 0,
                    ReadyTime = 0,
                    TruckList = [],
                    OrderServiceTime = 10,
                    OrderMinTime = 480,
                    OrderMaxTime = 960,
                },
            ],
        };
    }

    [Fact]
    public async Task OptimizeRequest_MakesAValidResponse()
    {
        // var client = _factory.CreateClient();

        // var project = CreateValidProject();
        // string content = JsonSerializer.Serialize(project);

        // var response = await client.PostAsync("/pvrpcloud/optimizerequest", new StringContent(content, Encoding.UTF8, "application/json"));
        // response.EnsureSuccessStatusCode();
        // Assert.True(true);
    }
}