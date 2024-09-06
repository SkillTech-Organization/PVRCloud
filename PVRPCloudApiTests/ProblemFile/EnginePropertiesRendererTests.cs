using FluentAssertions;
using PVRPCloud.ProblemFile;

namespace PVRPCloudApiTests.ProblemFile;

public class EnginePropertiesRendererTests
{
    [Fact]
    public void Render_CalledWithProjectName_ReturnsTheEngineConfigurationWithTheProjectName()
    {
        var result = new EnginePropertiesRenderer().Render("my project name");

        string expected = "setProblemFile(\"my project name\")\n" +
        "setEngineParameters(0, 1, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0)\n" +
        "runEngine()\n";

        result.ToString().Should().Be(expected);
    }
}
