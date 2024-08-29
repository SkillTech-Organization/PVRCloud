using FluentAssertions;
using PVRPCloud.ProblemFile;
using PVRPCloud.Requests;

namespace PVRPCloudApiTests.ProblemFile;

public class ProjectRendererTest
{
    [Fact]
    public void Render_ReturnsTheProjectAsTheProblemFile()
    {
        string result = new ProjectRenderer().Render(CreateProject());

        string expected = "setCustomerId(2000)\n" +
        "createCostProfile(5, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 0, 0, 0, 0)\n" +
        "createCostProfile(9, 34, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 79, 0, 0, 0, 0)\n" +
        """createTruckType("truck type name;KP1,ÉP1;3500;220;200")""" + "\n" +
        """createTruckType("another truck type;SP1,DP1;8000;300;200")""" + "\n" +
        """createClient("depot name", 12000000, 13000000)""" + "\n" +
        """createDepot("depot name", 1)""" + "\n" +
        "setDepotInformation(1, 1, 10, 15, 20, 0, 0, 0, 0)\n" +
        "createCapacityProfile(11, 22, 0, 0, 0)\n" +
        "createCapacityProfile(33, 44, 0, 0, 0)\n" +
        """createTruck(1, "truck name", 1, 1)""" + "\n"+
        "setTruckInformation(1, 1, 1, 10000000, 1, 10, 2, 9, 0, 0, 0)\n" +
        """createClient("client name", 19000000, 20000000)""" + "\n" +
        "setClientInformation(2, 55, 0, 0, 0, 0, 0)\n" +
        """createClient("client name 2", 44000000, 86000000)""" + "\n" +
        "setClientInformation(3, 19, 0, 0, 0, 0, 0)\n" +
        "createOrder(2)\n" +
        "setOrderInformation(1, 10, 20, 0, 0, 0, 12, 0, 0, 0, 0, 0)\n" +
        "setOrderServiceTime(1, 16)\n" +
        "addOrderTimeWindow(1, 4, 6)\n" +
        "addOrderTruck(1, 1)\n";

        result.Should().Be(expected);
    }

    private Project CreateProject()
    {
        return new()
        {
            MinTime = 20,
            CostProfiles = [
                new()
                {
                    ID = "cost profile id",
                    FixCost = 5,
                    KmCost = 11,
                    HourCost = 14
                },
                new()
                {
                    ID = "not used costprofile",
                    FixCost = 9,
                    KmCost = 34,
                    HourCost = 79
                },
            ],
            TruckTypes = [
                new()
                {
                    ID = "truck type id",
                    TruckTypeName = "truck type name",
                    RestrictedZones = ["KP1", "ÉP1"],
                    Weight = 3500,
                    XHeight = 220,
                    XWidth = 200,
                },
                new()
                {
                    ID = "not used truck type",
                    TruckTypeName = "another truck type",
                    RestrictedZones = ["SP1", "DP1"],
                    Weight = 8000,
                    XHeight = 300,
                    XWidth = 200,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "depot name",
                Lat = 12,
                Lng = 13,
                ServiceFixTime = 10,
                ServiceVarTime = 15,
            },
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id",
                    Capacity1 = 11,
                    Capacity2 = 22
                },
                new()
                {
                    ID = "not used capacity profile id",
                    Capacity1 = 33,
                    Capacity2 = 44
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck id",
                    TruckName = "truck name",
                    TruckTypeID = "truck type id",
                    CostProfileID = "cost profile id",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 10,
                    EarliestStart = 2,
                    LatestStart = 9,
                }
            ],
            Clients = [
                new()
                {
                    ID = "client id",
                    ClientName = "client name",
                    Lat = 19,
                    Lng = 20,
                    ServiceFixTime = 55,
                },
                new()
                {
                    ID = "not used client id",
                    ClientName = "client name 2",
                    Lat = 44,
                    Lng = 86,
                    ServiceFixTime = 19,
                }
            ],
            Orders = [
                new()
                {
                    ID = "order id",
                    ClientID = "client id",
                    Quantity1 = 10,
                    Quantity2 = 20,
                    ReadyTime = 12,
                    OrderServiceTime = 16,
                    OrderMinTime = 4,
                    OrderMaxTime = 6,
                    TruckIDs = ["truck id"],
                }
            ],
        };
    }
}
