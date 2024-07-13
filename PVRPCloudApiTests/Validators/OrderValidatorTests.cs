using FluentAssertions;
using FluentValidation;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class OrderValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1", "truck2"]
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
    public void Validate_IdIsNotValid_ThrowsValidationException(string? value)
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var act = () => sut.Validate(project.Orders[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_IdIsNotUnique_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1"]
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
                    TruckList = ["truck1"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var act = () => sut.Validate(project.Orders[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ClientIdNotFoud_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var act = () => sut.Validate(project.Orders[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ClientIdIsNotValid_ThrowsValidationException(string? value)
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var act = () => sut.Validate(project.Orders[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_OrderServiceTimeIsNegative_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var act = () => sut.Validate(project.Orders[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_OrderMinTimeIsNegative_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var act = () => sut.Validate(project.Orders[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_OrderMaxTimeIsNegative_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1", "truck2"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var act = () => sut.Validate(project.Orders[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_TruckIdsContainsInvalidElement_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    TruckList = ["truck1", "sajt"]
                }
            ]
        };

        OrderValidator sut = new(project);

        var act = () => sut.Validate(project.Orders[0]);

        act.Should().Throw<ValidationException>();
    }
}
