using FluentAssertions;
using PMapCore.BO;
using PVRPCloud;
using PVRPCloud.ProblemFile;
using PVRPCloud.Requests;

namespace PVRPCloudApiTests.ProblemFile;

public class ProjectRendererTest
{
    [Fact]
    public void Render_ReturnsTheProjectAsTheProblemFile()
    {
        string result = new ProjectRenderer().Render(Project, ClientPairs, Routes);

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
        "addOrderTruck(1, 1)\n" +
        "setRelationAccess(1, 1, 2, 5, 12)\n" +
        "setRelationAccess(2, 1, 2, 5, 12)\n" +
        "setRelationAccess(1, 2, 1, 5, 12)\n" +
        "setRelationAccess(2, 2, 1, 5, 12)\n" +
        """setProblemFile("test project")""" + "\n" +
        "setEngineParameters(0, 1, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0)\n" +
        "runEngine()\n";

        result.Should().Be(expected);
    }

    private Project Project => new()
    {
        ProjectName = "test project",
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
        TruckTypes = [TruckType, NotUsedTruckType],
        Depot = Depot,
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
            Client,
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

    private List<(ClientNodeIdPair From, ClientNodeIdPair To)> ClientPairs => [
        (new ClientNodeIdPair(Depot, 1), new ClientNodeIdPair(Client, 2)),
        (new ClientNodeIdPair(Client, 2), new ClientNodeIdPair(Depot, 1)),
    ];

    private PMapRoute[] Routes = [
        new()
        {
            fromNOD_ID = 1,
            toNOD_ID = 2,
            TruckTypeId = TruckType.ID,
            RZN_ID_LIST = string.Join(",", TruckType.RestrictedZones),
            GVWR = TruckType.Weight,
            Height = 0,
            Width = 0,
            route = new boRoute()
            {
                DST_DISTANCE = 5,
                Edges = [
                    new boEdge()
                    {
                        EDG_LENGTH = 1000,
                        RDT_VALUE = 1
                    }
                ]
            }
        },
        new()
        {
            fromNOD_ID = 1,
            toNOD_ID = 2,
            TruckTypeId = NotUsedTruckType.ID,
            RZN_ID_LIST = string.Join(",", NotUsedTruckType.RestrictedZones),
            GVWR = NotUsedTruckType.Weight,
            Height = 0,
            Width = 0,
            route = new boRoute()
            {
                DST_DISTANCE = 5,
                Edges = [
                    new boEdge()
                    {
                        EDG_LENGTH = 1000,
                        RDT_VALUE = 1
                    }
                ]
            }
        },
        new()
        {
            fromNOD_ID = 2,
            toNOD_ID = 1,
            TruckTypeId = TruckType.ID,
            RZN_ID_LIST = string.Join(",", TruckType.RestrictedZones),
            GVWR = TruckType.Weight,
            Height = 0,
            Width = 0,
            route = new boRoute()
            {
                DST_DISTANCE = 5,
                Edges = [
                    new boEdge()
                    {
                        EDG_LENGTH = 1000,
                        RDT_VALUE = 1
                    }
                ]
            }
        },
        new()
        {
            fromNOD_ID = 2,
            toNOD_ID = 1,
            TruckTypeId = NotUsedTruckType.ID,
            RZN_ID_LIST = string.Join(",", NotUsedTruckType.RestrictedZones),
            GVWR = NotUsedTruckType.Weight,
            Height = 0,
            Width = 0,
            route = new boRoute()
            {
                DST_DISTANCE = 5,
                Edges = [
                    new boEdge()
                    {
                        EDG_LENGTH = 1000,
                        RDT_VALUE = 1
                    }
                ]
            }
        }
    ];

    private static TruckType TruckType => new()
    {
        ID = "truck type id",
        TruckTypeName = "truck type name",
        RestrictedZones = ["KP1", "ÉP1"],
        Weight = 3500,
        XHeight = 220,
        XWidth = 200,
        SpeedValues = new Dictionary<int, int>
        {
            [1] = 5,
        }
    };

    private static TruckType NotUsedTruckType => new()
    {
        ID = "not used truck type",
        TruckTypeName = "another truck type",
        RestrictedZones = ["SP1", "DP1"],
        Weight = 8000,
        XHeight = 300,
        XWidth = 200,
        SpeedValues = new Dictionary<int, int>
        {
            [1] = 5,
        }
    };

    private Depot Depot => new()
    {
        ID = "depot id",
        DepotName = "depot name",
        Lat = 12,
        Lng = 13,
        ServiceFixTime = 10,
        ServiceVarTime = 15,
    };

    private Client Client => new()
    {
        ID = "client id",
        ClientName = "client name",
        Lat = 19,
        Lng = 20,
        ServiceFixTime = 55,
    };
}
