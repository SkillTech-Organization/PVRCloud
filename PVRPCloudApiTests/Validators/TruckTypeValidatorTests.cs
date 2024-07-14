using FluentAssertions;
using FluentValidation;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class TruckTypeValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_IdIsInvalid_ThrowsValidationException(string? value)
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = value!,
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var act = () => sut.Validate(project.TruckTypes[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_IdIsNotUnique_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "not unique id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                },
                new()
                {
                    ID = "not unique id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var act = () => sut.Validate(project.TruckTypes[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_TruckTypeNameIsInvalid_ThrowsValidationException(string? value)
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "id",
                    TruckTypeName = value!,
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var act = () => sut.Validate(project.TruckTypes[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_WeightIsNegative_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = -1,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var act = () => sut.Validate(project.TruckTypes[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_XHeightIsNegative_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = -1,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var act = () => sut.Validate(project.TruckTypes[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_XWidthIsNegative_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = -1,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var act = () => sut.Validate(project.TruckTypes[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    public void Validate_SpeedValuesIsOutOfRange_ThrowsValidationException(int value)
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "id",
                    TruckTypeName = "name",
                    RestrictedZones = ["P35"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [value] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var act = () => sut.Validate(project.TruckTypes[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_RestrictedZonesIsOutOfRange_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "id",
                    TruckTypeName = "name",
                    RestrictedZones = ["invalid value"],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var act = () => sut.Validate(project.TruckTypes[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_RestrictedZonesIsEmpty_ReturnsValidResult()
    {
        PVRPCloudProject project = new()
        {
            TruckTypes = [
                new()
                {
                    ID = "id",
                    TruckTypeName = "name",
                    RestrictedZones = [],
                    Weight = 0,
                    XHeight = 0,
                    XWidth = 0,
                    SpeedValues = new Dictionary<int, int>()
                    {
                        [1] = 70
                    }
                }
            ]
        };

        TruckTypeValidator sut = new(project);

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeTrue();
    }
}
