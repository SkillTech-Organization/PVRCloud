using FluentAssertions;
using FluentValidation.TestHelper;
using PVRPCommon;
using PVRPCommon.Models;
using PVRPCommon.Validators;

namespace PVRPCloudApiTests.Validators;

public class OrderValidatorTests
{
    private readonly ProjectValidator _projectValidator = new();

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

        OrderValidator sut = new();

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

        OrderValidator sut = new();

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
        var result = _projectValidator.TestValidate(project);

        result.ShouldHaveValidationErrorFor(x => x.Orders)
            .WithErrorMessage(Messages.ERR_ID_UNIQUE.Replace("{PropertyName}", "Orders"));
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

        var result = _projectValidator.TestValidate(project);

        result.ShouldHaveValidationErrorFor("ClientID[0]")
            .WithErrorMessage(Messages.ERR_NOT_FOUND
            .Replace("{PropertyName}", "ClientID")
            .Replace("{PropertyValue}", project.Orders[0].ClientID.ToString()));
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

        OrderValidator sut = new();

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

        OrderValidator sut = new();

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

        OrderValidator sut = new();

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

        OrderValidator sut = new();

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

        OrderValidator sut = new();

        var result = sut.Validate(project.Orders[0]);

        result.IsValid.Should().BeFalse();
    }
}
