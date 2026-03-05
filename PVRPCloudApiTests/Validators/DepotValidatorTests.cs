using FluentAssertions;
using FluentValidation.TestHelper;
using PVRPCommon.Models;
using PVRPCommon.Validators;

namespace PVRPCloudApiTests.Validators;

public class DepotValidatorTests
{
    private readonly ProjectValidator _validator = new();

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
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new();

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
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new();

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
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new();

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
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new();

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
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new();

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
                DepotMinTime = 0,
                DepotMaxTime = 0,
            }
        };

        DepotValidator sut = new();

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
                DepotMinTime = 1,
                DepotMaxTime = 0,
            }
        };

        var result = _validator.TestValidate(project);

        string expectedMessage = PVRPCommon.Messages.ERR_GREATER_THAN_OR_EQUAL
            .Replace("{0}", "DepotMinTime")
            .Replace("{1}", project.MinTime.ToString())
            .Replace("{2}", project.Depot.DepotMinTime.ToString());

        result.Errors.Should().Contain(e => e.ErrorMessage == expectedMessage);
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
                DepotMinTime = 0,
                DepotMaxTime = 3,
            }
        };

        var result = _validator.TestValidate(project);

        string expectedMessage = PVRPCommon.Messages.ERR_LESS_THAN_OR_EQUAL
            .Replace("{0}", "DepotMaxTime")
            .Replace("{1}", project.MaxTime.ToString())
            .Replace("{2}", project.Depot.DepotMaxTime.ToString());

        result.Errors.Should().Contain(e => e.ErrorMessage == expectedMessage);
    }
}
