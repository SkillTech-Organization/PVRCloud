using FluentAssertions;
using FluentValidation;
using PVRPCloud.Requests;
using PVRPCloudApi.Validators;

namespace PVRPCloudApiTests.Validators;

public class TruckValidatorTests
{
    [Fact]
    public void Validate_ReturnsValidResult()
    {
        PVRPCloudProject project = new()
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
                    ID = "id",
                    TruckName = "name",
                    TruckTypeID = "truck type id",
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                }
            ]
        };

        TruckValidator sut = new(project);

        var result = sut.Validate(project.Trucks[0]);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_IdIsInvalid_ThrowsValidationException(string? value)
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                }
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_IdIsNotUnique_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
                new()
                {
                    ID = "not unique id"
                }
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_TruckTypeIdIsNotValid_ThrowsValidationException(string? value)
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_TruckTypeIdIsNotReal_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_TruckNameIsNotValid_ThrowsValidationException(string? value)
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public void Validate_ArrDepotMaxTimeIsOutOfProjectTime_ThrowsValidationException(int value)
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = value,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_ArrDepotMaxTimeIsZero_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 0,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_MaxWorkTimeIsZero_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 0,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_EarliestStartIsNegative_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = -1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_LatestStartIsLessThenEarliestStart_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 2,
                    LatestStart = 1,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_LatestStartIsGreaterThanMaxTime_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "capacity profile id",
                    MaxWorkTime = 1,
                    EarliestStart = 2,
                    LatestStart = 4,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_CapacityProfileIdIsNotValid_ThrowsValidationException(string? value)
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = value!,
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Validate_CapacityProfileIdIsNotFound_ThrowsValidationException()
    {
        PVRPCloudProject project = new()
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
                    ArrDepotMaxTime = 1,
                    CapacityProfileID = "not valuid",
                    MaxWorkTime = 1,
                    EarliestStart = 1,
                    LatestStart = 2,
                },
            ]
        };

        TruckValidator sut = new(project);

        var act = () => sut.Validate(project.Trucks[0]);

        act.Should().Throw<ValidationException>();
    }
}
