using FluentAssertions;
using PVRPCloud.ProblemFile;

namespace PVRPCloudApiTests.ProblemFile;

public class SetCustomerIdRendererTest
{
    [Fact]
    public void Run_ReturnsCorrectString()
    {
        string result = new SetCustomerIdRenderer().Render();

        result.Should().Be("setCustomerId(2000)");
    }
}
