using System.Net;
using FluentAssertions;

namespace PVRPCloudApiTests;

public class PVRPCloudResultTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private const string Endpoint = "/v1/PVRPCloudResult/";

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PVRPCloudResult_ExistingId_ReturnsOk()
    {
        string requestId = "12345678";

        var response = await _client.GetAsync(Endpoint + requestId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PVRPCloudResult_NonExistingId_ReturnsNotFound()
    {
        string requestId = "1234567";

        var response = await _client.GetAsync(Endpoint + requestId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
