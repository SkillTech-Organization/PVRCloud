using PVRPCloud.Models;

namespace PVRPCloudApiTests;

public static class ProjectFactory
{
    public static Project CreateValidProject()
    {
        return new()
        {
            ProjectName = "name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 5,
            DistanceLimit = 2,
            CostProfiles = [
                new()
                {
                    ID = "cost profile ID",
                    FixCost = 100,
                    HourCost = 1000,
                    KmCost = 200
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id",
                    Capacity1 = 2,
                    Capacity2 = 4,
                }
            ],
            TruckTypes = [
                new()
                {
                    ID = "truck type id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    CostProfileID = "cost profile ID",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "depot name",
                Lat = 0.01,
                Lng = 0.02,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 1,
                DepotMaxTime = 0,
            },
            Clients = [
                new()
                {
                    ID = "client id",
                    ClientName = "client name",
                    Lat = 0.003,
                    Lng = 0.02,
                    ServiceFixTime = 0
                }
            ],
            Orders = [
                new()
                {
                    ID = "order id",
                    ClientID = "client id",
                    Quantity1 = 1.1,
                    Quantity2 = 2,
                    ReadyTime = 2,
                    OrderServiceTime = 1,
                    OrderMinTime = 1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck id"]
                }
            ]
        };
    }

    public static Project CreateInvalidProject()
    {
        return new()
        {
            ProjectName = null!,
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 5,
            DistanceLimit = 2,
            CostProfiles = [
                new()
                {
                    ID = "cost profile ID",
                    FixCost = 100,
                    HourCost = 1000,
                    KmCost = 200
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id",
                    Capacity1 = 2,
                    Capacity2 = 4,
                }
            ],
            TruckTypes = [
                new()
                {
                    ID = "truck type id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 1,
                DepotMaxTime = 0,
            },
            Clients = [
                new()
                {
                    ID = "client id",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ],
            Orders = [
                new()
                {
                    ID = "order id",
                    ClientID = "client id",
                    Quantity1 = 1.1,
                    Quantity2 = 2,
                    ReadyTime = 2,
                    OrderServiceTime = 1,
                    OrderMinTime = 1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck id"]
                }
            ]
        };
    }
}
