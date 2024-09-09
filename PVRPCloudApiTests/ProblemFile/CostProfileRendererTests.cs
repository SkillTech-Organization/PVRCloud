using FluentAssertions;
using PVRPCloud.ProblemFile;
using PVRPCloud.Models;

namespace PVRPCloudApiTests.ProblemFile;

public class CostProfileRendererTests
{
    private readonly CostProfileRenderer _sut = new();

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

        result.ToString().Should().Contain("createCostProfile(5, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0)");
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
                ID = "cost profile id2",
                FixCost = 20,
                HourCost = 21,
                KmCost = 22,
            }
        ]);

        var expected = $"createCostProfile(10, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 0){Environment.NewLine}createCostProfile(20, 22, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 21, 0, 0, 0, 0){Environment.NewLine}";

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
        expectedCostProfile.Key.Should().Be("cost profile id");
        expectedCostProfile.Value.Should().Be(1);
    }
}
