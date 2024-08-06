using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PVRPCloudApiTests;

public class OptimizeRequestTests(WebApplicationFactory<Program> _factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Endpoint = "/v1/PVRPCloudRequest";

    private readonly HttpClient client = _factory.CreateClient();

    [Fact]
    public async Task PVRPCloudRequest_ValidInput_MakesAValidResponse()
    {
        var project = ProjectFactory.CreateValidProject();
        string content = JsonSerializer.Serialize(project);

        var response = await client.PostAsync(Endpoint, new StringContent(content, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task PVRPCloudRequest_InvalidInput_ReturnsBadRequest()
    {
        var project = ProjectFactory.CreateInvalidProject();
        string content = JsonSerializer.Serialize(project);

        var response = await client.PostAsync(Endpoint, new StringContent(content, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}