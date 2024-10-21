using FluentAssertions;
using PVRPCloud.Models;
using PVRPCloud.ProblemFile;

namespace PVRPCloudApiTests.ProblemFile;

public class CapacityProfileRendererTests
{
    private readonly CapacityProfileRenderer _sut = new();

    [Fact]
    public void Render_CalledWithCapacityProfile_CreatesACapacityProfileSection()
    {
        CapacityProfile cp = new()
        {
            ID = "capacity profile id",
            Capacity1 = 10,
            Capacity2 = 20,
        };

        var result = _sut.Render([cp]);

        result.ToString().Should().Contain("createCapacityProfile(10000, 20, 0, 0, 0)");
    }

    [Fact]
    public void Render_CalledWithCapacityProfile_CreatesAnEntryInCapacityProfiles()
    {
        CapacityProfile cp = new()
        {
            ID = "capacity profile id",
            Capacity1 = 10,
            Capacity2 = 20,
        };

        _ = _sut.Render([cp]);

        _sut.Profiles.Count.Should().Be(1);

        var entry = _sut.Profiles.First();
        entry.Key.Should().Be("capacity profile id");
        entry.Value.Should().Be(1);
    }
}
