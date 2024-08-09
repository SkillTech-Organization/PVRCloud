using FluentAssertions;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class ClientValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = "client id",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IdIsNull_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = null!,
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_IdIsEmpty_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = string.Empty,
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

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
                    ID = "not unique id",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0
                },
                new()
                {
                    ID = "not unique id",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ClientNameIsEmpty_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = string.Empty,
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ClientNameIsNull_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = null!,
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-90)]
    [InlineData(0)]
    [InlineData(90)]
    public void Validate_LatIsBetweenNegative90And90_ReturnsValidResult(int value)
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = value,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Validate_LatIsOutsideNegative90And90_ReturnsInvalidResult(double value)
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = value,
                    Lng = 0,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-180)]
    [InlineData(0)]
    [InlineData(190)]
    public void Validate_LngIsBetweenNegative180And190_ReturnsValidResult(int value)
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = 0,
                    Lng = value,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(191)]
    public void Validate_LatIsOutsideNegative181And191_ReturnsInvalidResult(double value)
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = 0,
                    Lng = value,
                    ServiceFixTime = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ServiceFixTimeIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    ServiceFixTime = -1
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeFalse();
    }
}
