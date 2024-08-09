using FluentAssertions;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class CapacityProfileValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        Project project = new()
        {
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id",
                    Capacity1 = 0,
                    Capacity2 = 2,
                }
            ]
        };
        CapacityProfileValidator sut = new(project);

        var result = sut.Validate(project.CapacityProfiles[0]);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IdIsNull_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CapacityProfiles = [
                new()
                {
                    ID = null!,
                    Capacity1 = 0,
                    Capacity2 = 2,
                }
            ]
        };
        CapacityProfileValidator sut = new(project);

        var result = sut.Validate(project.CapacityProfiles[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_IdIsEmpty_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CapacityProfiles = [
                new()
                {
                    ID = string.Empty,
                    Capacity1 = 0,
                    Capacity2 = 2,
                }
            ]
        };
        CapacityProfileValidator sut = new(project);

        var result = sut.Validate(project.CapacityProfiles[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_IdIsRepeated_ReturnsInvalidResult()
    {
        Project project = new()
        {
            CapacityProfiles = [
                new()
                {
                    ID = "not unique id",
                    Capacity1 = 0,
                    Capacity2 = 2,
                },
                new()
                {
                    ID = "not unique id",
                    Capacity1 = 0,
                    Capacity2 = 2,
                }
            ]
        };
        CapacityProfileValidator sut = new(project);

        var result = sut.Validate(project.CapacityProfiles[0]);

        result.IsValid.Should().BeFalse();
    }
}
