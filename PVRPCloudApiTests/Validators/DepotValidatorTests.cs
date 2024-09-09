using FluentAssertions;
using PVRPCloud.Models;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class DepotValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        Project project = new()
        {
            Depot = new()
            {
                ID = "id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_IdIsInvalid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            Depot = new()
            {
                ID = value!,
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_DepotNameIsInvalid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            Depot = new()
            {
                ID = "id",
                DepotName = value!,
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Validate_LatIsInvalid_ReturnsInvalidResult(double value)
    {
        Project project = new()
        {
            Depot = new()
            {
                ID = "id",
                DepotName = "name",
                Lat = value,
                Lng = 0,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(191)]
    public void Validate_LngIsInvalid_ReturnsInvalidResult(double value)
    {
        Project project = new()
        {
            Depot = new()
            {
                ID = "id",
                DepotName = "name",
                Lat = 0,
                Lng = value,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ServiceFixTimeIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Depot = new()
            {
                ID = "id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = -1,
                ServiceVarTime = 0,
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ServiceVarTimeIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            Depot = new()
            {
                ID = "id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
                ServiceVarTime = -1,
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_DepotMinTimeIsLessThanProjectMinTime_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 2,
            Depot = new()
            {
                ID = "id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 1,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_DepotMaxTimeIsGreaterThanProjectMinTime_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MaxTime = 2,
            Depot = new()
            {
                ID = "id",
                DepotName = "name",
                Lat = 0,
                Lng = 0,
                ServiceFixTime = 0,
                ServiceVarTime = 0,
                DepotMinTime = 0,
                DepotMaxTime = 3,
            }
        };

        DepotValidator sut = new(project);

        var result = sut.Validate(project.Depot);

        result.IsValid.Should().BeFalse();
    }
}
