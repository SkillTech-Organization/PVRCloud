﻿using FluentAssertions;
using FluentValidation;
using PVRPCloud.Models;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class ProjectValidatorTests
{
    [Fact]
    public void Validate_Project_ResturnsValidResult()
    {
        Project project = new()
        {
            ProjectName = "name",
            ProjectDate = DateTime.Parse("2024-05-11"),
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var result = sut.Validate(project);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ProjectDateIsInvalid_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "name",
            // ProjectDate =
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ProjectNameIsInvalid_ThrowsValidationException(string? value)
    {
        Project project = new()
        {
            ProjectName = value!,
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
             }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void Validate_MaxTimeIsLessThanMinTime_ThrowsValidationException(int value)
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = value,
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_MaxTourDurationIsLessThan1_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 0,
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
       }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_DistanceLimitIsNegative_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 1,
            DistanceLimit = -1,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_CostProfilesIsEmpty_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 1,
            DistanceLimit = 1,
            CostProfiles = [],
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_CapacityProfilesIsEmpty_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 1,
            DistanceLimit = 1,
            CostProfiles = [
                new()
                {
                    ID = "cost profile ID",
                    FixCost = 100,
                    HourCost = 1000,
                    KmCost = 200
                }
            ],
            CapacityProfiles = [],
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_TruckTypesIsEmpty_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 1,
            DistanceLimit = 1,
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
            TruckTypes = [],
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_TrucksIsEmpty_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 1,
            DistanceLimit = 1,
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
            Trucks = [],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_DepotIsNull_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 1,
            DistanceLimit = 1,
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               }
            ],
            Depot = null!,
            Clients = [
                new()
                {
                    ID = "client id",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ClientsIsEmpty_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 1,
            DistanceLimit = 1,
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
                DepotMinTime = 1,
                DepotMaxTime = 0,
            },
            Clients = [],
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

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_OrdersIsEmpty_ThrowsValidationException()
    {
        Project project = new()
        {
            ProjectName = "project name",
            MinTime = 1,
            MaxTime = 4,
            MaxTourDuration = 1,
            DistanceLimit = 1,
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
                    ETollCat = 2,
                    EnvironmentalClass = 4,
                }
            ],
            Depot = new()
            {
                ID = "depot id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
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
                    ServiceFixTime = 0,
                    Quantity1SrerviceInSec = 0
               }
            ],
            Orders = []
        };

        ProjectValidator sut = new();

        var act = () => sut.Validate(project);

        act.Should().Throw<ValidationException>();
    }
}
