using FluentAssertions;
using PVRPCloud.ProblemFile;

namespace PVRPCloudApiTests.ProblemFile;

public class EnginePropertiesRendererTests
{
    [Fact]
    public void Render_CalledWithProjectName_ReturnsTheEngineConfigurationWithTheProjectName()
    {
        string result = new EnginePropertiesRenderer().Render("my project name");

        string expected = """
        setProblemFile("my project name")
        setEngineParameters(0, 1, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0)
        runEngine()
        """;

        result.Should().Be(expected);
    }
}
