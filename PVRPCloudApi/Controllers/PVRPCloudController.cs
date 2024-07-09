using Microsoft.AspNetCore.Mvc;
using PVRPCloud;
using PVRPCloud.Requests;

namespace PVRPCloudApi.Controllers;

[ApiController]
[Route("[action]")]
public class PVRPCloudController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<PVRPCloudResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PVRPCloudRequest(PVRPCloudProject request)
    {
        return Accepted(new PVRPCloudResponse
        {
            RequestID = "12345678",
            Results = [ PVRPCloud.PVRPCloudResult.Success(request) ]
        });
    }

    [HttpGet("{requestId}")]
    [ProducesResponseType<PVRPCloudQueueResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PVRPCloudResult(string requestId)
    {
        var exampleResponse = new PVRPCloudProjectRes()
        {
            Projectname = "PRJ01",
            MinTime = DateTime.Now.AddDays(-5),
            MaxTime = DateTime.Now.AddDays(2),
            PlanTours = [
                new()
                {
                    Truck = new PVRPCloud.Requests.PVRPCloudTruck
                    {
                        ID = "valami Id",
                        TruckTypeID = "truck type Id",
                        TruckName = "IFA",
                        ArrDepotMaxTime = 3,
                        CapacityProfileID = "id",
                        MaxWorkTime = 3,
                        EarliestStart = 2,
                        LatestStart = 2,
                    },
                    StartTime = DateTime.Now.AddDays(-5),
                    EndTime = DateTime.Now.AddDays(1),
                    TourLength = 5,
                    TourToll = 5,
                    Route = "string",
                    TourPoints = [
                        new PVRPCloudTourPoint()
                        {
                            Depot = new()
                            {
                                ID = "ids",
                                DepotName = "WH1",
                                Lat = 1,
                                Lng = 1,
                                DepotMinTime = 23,
                                DepotMaxTime = 24,
                                ServiceFixTime = 2,
                                ServiceVarTime = 3
                            },
                            Client = new()
                            {
                                ID = "ID",
                                ClientName = "Client name",
                                Lat = 2,
                                Lng = 2,
                                FixService = 2
                            },
                            Lat = 0,
                            Lng = 0,
                            TourPointNo = 5,
                            Distance = 5,
                            Duration = 2,
                            ArrTime = DateTime.Now,
                            ServTime = DateTime.Now,
                            epTime = DateTime.Now,
                            Order = new(),
                            Qty1 = 1,
                            Qty2 = 1,
                            Qty3 = 1,
                            Qty4 = 1,
                            Qty5 = 1,
                        },
                        new PVRPCloudTourPoint()
                        {
                            Depot = new()
                            {
                                ID = "ids",
                                DepotName = "WH1",
                                Lat = 1,
                                Lng = 1,
                                DepotMinTime = 23,
                                DepotMaxTime = 24,
                                ServiceFixTime = 2,
                                ServiceVarTime = 3
                            },
                            Client = new()
                            {
                                ID = "ID",
                                ClientName = "Client name",
                                Lat = 2,
                                Lng = 2,
                                FixService = 2
                            },
                            Lat = 0,
                            Lng = 0,
                            TourPointNo = 5,
                            Distance = 5,
                            Duration = 2,
                            ArrTime = DateTime.Now,
                            ServTime = DateTime.Now,
                            epTime = DateTime.Now,
                            Order = new(),
                            Qty1 = 1,
                            Qty2 = 1,
                            Qty3 = 1,
                            Qty4 = 1,
                            Qty5 = 1,
                        }
                    ]
                },
                new()
                {
                    Truck = new PVRPCloud.Requests.PVRPCloudTruck
                    {
                        ID = "valami Id",
                        TruckTypeID = "truck type Id",
                        TruckName = "IFA",
                        ArrDepotMaxTime = 3,
                        CapacityProfileID = "id",
                        MaxWorkTime = 3,
                        EarliestStart = 2,
                        LatestStart = 2,
                    },
                    StartTime = DateTime.Now.AddDays(-5),
                    EndTime = DateTime.Now.AddDays(1),
                    TourLength = 5,
                    TourToll = 5,
                    Route = "string",
                    TourPoints = [
                        new PVRPCloudTourPoint()
                        {
                            Depot = new()
                            {
                                ID = "ids",
                                DepotName = "WH1",
                                Lat = 1,
                                Lng = 1,
                                DepotMinTime = 23,
                                DepotMaxTime = 24,
                                ServiceFixTime = 2,
                                ServiceVarTime = 3
                            },
                            Client = new()
                            {
                                ID = "ID",
                                ClientName = "Client name",
                                Lat = 2,
                                Lng = 2,
                                FixService = 2
                            },
                            Lat = 0,
                            Lng = 0,
                            TourPointNo = 5,
                            Distance = 5,
                            Duration = 2,
                            ArrTime = DateTime.Now,
                            ServTime = DateTime.Now,
                            epTime = DateTime.Now,
                            Order = new(),
                            Qty1 = 1,
                            Qty2 = 1,
                            Qty3 = 1,
                            Qty4 = 1,
                            Qty5 = 1,
                        },
                        new PVRPCloudTourPoint()
                        {
                            Depot = new()
                            {
                                ID = "ids",
                                DepotName = "WH1",
                                Lat = 1,
                                Lng = 1,
                                DepotMinTime = 23,
                                DepotMaxTime = 24,
                                ServiceFixTime = 2,
                                ServiceVarTime = 3
                            },
                            Client = new()
                            {
                                ID = "ID",
                                ClientName = "Client name",
                                Lat = 2,
                                Lng = 2,
                                FixService = 2
                            },
                            Lat = 0,
                            Lng = 0,
                            TourPointNo = 5,
                            Distance = 5,
                            Duration = 2,
                            ArrTime = DateTime.Now,
                            ServTime = DateTime.Now,
                            epTime = DateTime.Now,
                            Order = new(),
                            Qty1 = 1,
                            Qty2 = 1,
                            Qty3 = 1,
                            Qty4 = 1,
                            Qty5 = 1,
                        }
                    ]
                }
            ],
            UnplannedOrders = [
                new()
                {
                    Qty1 = 1,
                    Qty2 = 1,
                    Qty3 = 1,
                    Qty4 = 1,
                    Qty5 = 1,
                },
                new()
                {
                    Qty1 = 1,
                    Qty2 = 1,
                    Qty3 = 1,
                    Qty4 = 1,
                    Qty5 = 1,
                }
            ],
            CalcInput = "Ez nem tudom mi",
            CalcOutput = "Ezt sem tudom hogy mi :)",
            PVRPConsole = "Console"
        };
        return Ok(new PVRPCloudResponse
        {
            RequestID = requestId,
            Results = [
                PVRPCloud.PVRPCloudResult.Success(exampleResponse)
            ]
        });
    }
}
