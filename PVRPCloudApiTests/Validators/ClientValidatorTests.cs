using FluentAssertions;
using FluentValidation;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class ClientValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = "client id",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    FixService = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        var result = sut.Validate(project.Clients[0]);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IdIsNull_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = null!,
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    FixService = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        Action act = () => sut.Validate(project.Clients[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_IdIsEmpty_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = string.Empty,
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    FixService = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        Action act = () => sut.Validate(project.Clients[0]);

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
                    ID = "not unique id",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    FixService = 0
                },
                new()
                {
                    ID = "not unique id",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    FixService = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        Action act = () => sut.Validate(project.Clients[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ClientNameIsEmpty_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = string.Empty,
                    Lat = 0,
                    Lng = 0,
                    FixService = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        Action act = () => sut.Validate(project.Clients[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ClientNameIsNull_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = null!,
                    Lat = 0,
                    Lng = 0,
                    FixService = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        Action act = () => sut.Validate(project.Clients[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(-90)]
    [InlineData(0)]
    [InlineData(90)]
    public void Validate_LatIsBetweenNegative90And90_ReturnsValidResult(int value)
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = value,
                    Lng = 0,
                    FixService = 0
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
    public void Validate_LatIsOutsideNegative90And90_ThrowsValidationException(double value)
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = value,
                    Lng = 0,
                    FixService = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        Action act = () => sut.Validate(project.Clients[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(-180)]
    [InlineData(0)]
    [InlineData(190)]
    public void Validate_LngIsBetweenNegative180And190_ReturnsValidResult(int value)
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = 0,
                    Lng = value,
                    FixService = 0
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
    public void Validate_LatIsOutsideNegative181And191_ThrowsValidationException(double value)
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = 0,
                    Lng = value,
                    FixService = 0
                }
            ]
        };

        ClientValidator sut = new(project);

        Action act = () => sut.Validate(project.Clients[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_FixServiceIsNegative_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            Clients = [
                new()
                {
                    ID = "valami",
                    ClientName = "name",
                    Lat = 0,
                    Lng = 0,
                    FixService = -1
                }
            ]
        };

        ClientValidator sut = new(project);

        Action act = () => sut.Validate(project.Clients[0]);

        act.Should().Throw<ValidationException>();
    }
}
