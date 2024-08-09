using FluentAssertions;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class OrderValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
                new()
                {
                    ID = "truck2"
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
                    TruckIDs = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_IdIsNotValid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
            ],
            Orders = [
                new()
                {
                    ID = value!,
                    ClientID = "client id",
                    Quantity1 = 1.1,
                    Quantity2 = 2,
                    ReadyTime = 2,
                    OrderServiceTime = 1,
                    OrderMinTime = 1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck1"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_IdIsNotUnique_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
            ],
            Orders = [
                new()
                {
                    ID = "not unique",
                    ClientID = "client id",
                    Quantity1 = 1.1,
                    Quantity2 = 2,
                    ReadyTime = 2,
                    OrderServiceTime = 1,
                    OrderMinTime = 1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck1"]
                },
                new()
                {
                    ID = "not unique",
                    ClientID = "client id",
                    Quantity1 = 1.1,
                    Quantity2 = 2,
                    ReadyTime = 2,
                    OrderServiceTime = 1,
                    OrderMinTime = 1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck1"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ClientIdNotFoud_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
                new()
                {
                    ID = "truck2"
                }
            ],
            Orders = [
                new()
                {
                    ID = "order id",
                    ClientID = "not valid client id",
                    Quantity1 = 1.1,
                    Quantity2 = 2,
                    ReadyTime = 2,
                    OrderServiceTime = 1,
                    OrderMinTime = 1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ClientIdIsNotValid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
                new()
                {
                    ID = "truck2"
                }
            ],
            Orders = [
                new()
                {
                    ID = "order id",
                    ClientID = value!,
                    Quantity1 = 1.1,
                    Quantity2 = 2,
                    ReadyTime = 2,
                    OrderServiceTime = 1,
                    OrderMinTime = 1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_OrderServiceTimeIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
                new()
                {
                    ID = "truck2"
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
                    OrderServiceTime = -1,
                    OrderMinTime = 1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_OrderMinTimeIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
                new()
                {
                    ID = "truck2"
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
                    OrderMinTime = -1,
                    OrderMaxTime = 1,
                    TruckIDs = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_OrderMaxTimeIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
                new()
                {
                    ID = "truck2"
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
                    OrderMaxTime = -1,
                    TruckIDs = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TruckIDsContainsInvalidElement_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID  = "client id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "truck1"
                },
                new()
                {
                    ID = "truck2"
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
                    OrderMaxTime = -1,
                    TruckIDs = ["truck1", "sajt"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }
}
