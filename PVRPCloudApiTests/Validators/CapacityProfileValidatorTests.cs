using FluentAssertions;
using FluentValidation.TestHelper;
using PVRPCommon.Models;
using PVRPCommon.Validators;

namespace PVRPCloudApiTests.Validators;

public class CapacityProfileValidatorTests
{

    private readonly ProjectValidator _projectValidator = new();

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
        CapacityProfileValidator sut = new();

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
        var result = _projectValidator.TestValidate(project);

        result.ShouldHaveValidationErrorFor("CapacityProfiles[0].ID")
            .WithErrorMessage(PVRPCommon.Messages.ERR_EMPTY.Replace("{PropertyName}", "ID"));
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

        var result = _projectValidator.TestValidate(project);

        result.ShouldHaveValidationErrorFor("CapacityProfiles[0].ID")
            .WithErrorMessage(PVRPCommon.Messages.ERR_MANDATORY.Replace("{PropertyName}", "ID"));
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


        var result = _projectValidator.TestValidate(project);

        result.ShouldHaveValidationErrorFor(x => x.CapacityProfiles)
            .WithErrorMessage(PVRPCommon.Messages.ERR_ID_UNIQUE.Replace("{PropertyName}", "Capacity Profiles"));
    }
}
