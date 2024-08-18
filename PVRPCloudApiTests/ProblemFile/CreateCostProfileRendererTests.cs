using FluentAssertions;
using PVRPCloud.ProblemFile;
using PVRPCloud.Requests;

namespace PVRPCloudApiTests.ProblemFile;

public class CreateCostProfileRendererTests
{
    private readonly CreateCostProfileRenderer _sut = new();

    [Fact]
    public void Render_PassingEmptyCollection_ReturnsEmptyStringBuilder()
    {
        var result = _sut.Render([]);

        result.ToString().Should().Be("");
    }

    [Fact]
    public void Render_PassingACostProfile_ReturnsCorrectStringBuilder()
    {
        var result = _sut.Render([
            new()
            {
                ID = "cost profile id",
                FixCost = 5,
                HourCost = 6,
                KmCost = 8,
            }
        ]);

        result.ToString().Should().Be("createCostProfile(5, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0)\n");
    }

    [Fact]
    public void Render_PassingMultipleCostProfiles_ReturnsCorrectStringBuilder()
    {
        var result = _sut.Render([
            new()
            {
                ID = "cost profile id",
                FixCost = 10,
                HourCost = 11,
                KmCost = 12,
            },
            new()
            {
                ID = "cost profile id",
                FixCost = 20,
                HourCost = 21,
                KmCost = 22,
            }
        ]);

        var expected = "createCostProfile(10, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0)\ncreateCostProfile(20, 22, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 21, 0, 0, 0, 0)\n";

        result.ToString().Should().Be(expected);
    }

    [Fact]
    public void Render_PassingACostProfile_GeneratesPVRPIdForCostProfile()
    {
        CostProfile costProfile = new()
        {
            ID = "cost profile id",
            FixCost = 5,
            HourCost = 6,
            KmCost = 8,
        };

        _ = _sut.Render([costProfile]);

        _sut.CostProfiles.Count.Should().Be(1);

        var expectedCostProfile = _sut.CostProfiles.First();
        expectedCostProfile.Key.Should().Be(1);
        expectedCostProfile.Value.Should().BeSameAs(costProfile);
    }
}
