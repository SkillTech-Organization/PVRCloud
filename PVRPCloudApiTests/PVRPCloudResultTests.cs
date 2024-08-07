using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PVRPCloudApiTests;

public class PVRPCloudResultTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
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
