using FluentAssertions;
using PVRPCloud.Models;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class CostProfileValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        Project project = new()
        {
            CostProfiles = [
                new()
                {
                    ID = "id",
                    FixCost = 0,
                    HourCost = 0,
                    KmCost = 0,
                }
            ]
        };

        CostProfileValidator sut = new(project);

        var result = sut.Validate(project.CostProfiles[0]);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IdIsNull_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CostProfiles = [
                new()
                {
                    ID = null!,
                    FixCost = 0,
                    HourCost = 0,
                    KmCost = 0,
                }
            ]
        };

        CostProfileValidator sut = new(project);

        var result = sut.Validate(project.CostProfiles[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_IdIsEmpty_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CostProfiles = [
                new()
                {
                    ID = string.Empty,
                    FixCost = 0,
                    HourCost = 0,
                    KmCost = 0,
                }
            ]
        };

        CostProfileValidator sut = new(project);

        var result = sut.Validate(project.CostProfiles[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_IdIsNotUnique_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CostProfiles = [
                new()
                {
                    ID = "not unique id",
                    FixCost = 0,
                    HourCost = 0,
                    KmCost = 0,
                },
                new()
                {
                    ID = "not unique id",
                    FixCost = 0,
                    HourCost = 0,
                    KmCost = 0,
                }
            ]
        };

        CostProfileValidator sut = new(project);

        var result = sut.Validate(project.CostProfiles[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_FixCostIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CostProfiles = [
                new()
                {
                    ID = string.Empty,
                    FixCost = -1,
                    HourCost = 0,
                    KmCost = 0,
                }
            ]
        };

        CostProfileValidator sut = new(project);

        var result = sut.Validate(project.CostProfiles[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_HourCostIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CostProfiles = [
                new()
                {
                    ID = string.Empty,
                    FixCost = 0,
                    HourCost = -1,
                    KmCost = 0,
                }
            ]
        };

        CostProfileValidator sut = new(project);

        var result = sut.Validate(project.CostProfiles[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_KmCostIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CostProfiles = [
                new()
                {
                    ID = string.Empty,
                    FixCost = 0,
                    HourCost = 0,
                    KmCost = -1,
                }
            ]
        };

        CostProfileValidator sut = new(project);

        var result = sut.Validate(project.CostProfiles[0]);

        result.IsValid.Should().BeFalse();
    }
}
