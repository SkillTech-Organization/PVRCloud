using FluentAssertions;
using FluentValidation;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class CapacityProfileValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        PVRPCloudProject project = new()
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
    public void Validate_IdIsNull_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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

        Action act = () => sut.Validate(project.CapacityProfiles[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_IdIsEmpty_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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

        Action act = () => sut.Validate(project.CapacityProfiles[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_IdIsRepeated_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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

        Action act = () => sut.Validate(project.CapacityProfiles[0]);

        act.Should().Throw<ValidationException>();
    }
}
