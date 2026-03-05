using FluentAssertions;
using FluentValidation.TestHelper;
using PVRPCommon;
using PVRPCommon.Models;
using PVRPCommon.Validators;

namespace PVRPCloudApiTests.Validators;

public class TruckValidatorTests
{
    private readonly ProjectValidator _projectValidator = new();
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            CostProfiles = [
                new()
                {
                    ID = "cost profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    CostProfileID = "cost profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               }
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_IdIsInvalid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = value!,
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
                }
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_IdIsNotUnique_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
                },
                new()
                {
                    ID = "not unique id"
                }
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_TruckTypeIdIsNotValid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "name",
                    TruckTypeID = value!,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
            },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TruckTypeIdIsNotReal_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "name",
                    TruckTypeID = "not valid",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_TruckNameIsNotValid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = value!,
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_MaxWorkTimeIsZero_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 1,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "truck name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 0,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EarliestStartIsNegative_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 1,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "truck name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = -1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_LatestStartIsLessThenEarliestStart_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 1,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "truck name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 2,
                    LatestStart = 1,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_LatestStartIsGreaterThanMaxTime_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 1,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "truck name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 2,
                    LatestStart = 4,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_CapacityProfileIdIsNotValid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = value!,
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_CapacityProfileIdIsNotFound_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "not valuid",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_CostProfileIdIsNotValid_ReturnsInvalidResult(string? value)
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            CostProfiles = [
                new()
                {
                    ID = "cost profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    CostProfileID = value!,
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        TruckValidator sut = new();

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_CostProfileIdIsNotFound_ReturnsInvalidResult()
    {
        Project project = new()
        {
            MinTime = 0,
            MaxTime = 3,
            TruckTypes = [
                new()
                {
                    ID = "truck type id"
                }
            ],
            CapacityProfiles = [
                new()
                {
                    ID = "capacity profile id"
                }
            ],
            CostProfiles = [
                new()
                {
                    ID = "cost profile id"
                }
            ],
            Trucks = [
                new()
                {
                    ID = "not unique id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    CapacityProfileID = "capacity profile id",
                    CostProfileID = "not valid",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                    ETollCat = 2,
                    EnvironmentalClass = 4,
               },
            ]
        };

        var result = _projectValidator.TestValidate(project);

        result.ShouldHaveValidationErrorFor("CostProfileID[0]")
            .WithErrorMessage(Messages.ERR_NOT_FOUND
            .Replace("{PropertyName}", "CostProfileID")
            .Replace("{PropertyValue}", project.Trucks[0].CostProfileID.ToString()));
    }
}
