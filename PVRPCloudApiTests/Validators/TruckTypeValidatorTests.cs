using FluentAssertions;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class TruckTypeValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        Project project = new()
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
    public void Validate_IdIsInvalid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
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

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_IdIsNotUnique_ReturnsInvalidResult()
    {
        Project project = new()
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

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_TruckTypeNameIsInvalid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
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

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WeightIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
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

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_XHeightIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
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

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_XWidthIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
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

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    public void Validate_SpeedValuesIsOutOfRange_ReturnsInvalidResult(int value)
    {
        Project project = new()
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

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_RestrictedZonesIsOutOfRange_ReturnsInvalidResult()
    {
        Project project = new()
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

        var result = sut.Validate(project.TruckTypes[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_RestrictedZonesIsEmpty_ReturnsValidResult()
    {
        Project project = new()
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
