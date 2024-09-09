using System.Collections.Frozen;
using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using GMap.NET;
using NSubstitute;
using PMapCore.BO;

namespace PVRPCloudApiTests;

public class PVRPCloudRequestTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private const string Endpoint = "/v1/PVRPCloudRequest";

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PVRPCloudRequest_ValidInput_MakesAValidResponse()
    {
        Dictionary<string, boEdge> edges = new()
        {
            ["valami"] = new()
            {
                RDT_VALUE = 3,
                NOD_ID_FROM = 6,
                NOD_ID_TO = 7
            },
        };
        factory.RouteData.Edges.Returns(edges.ToFrozenDictionary());

        Dictionary<int, PointLatLng> nodePositions = new()
        {
            [7] = new()
        };
        factory.RouteData.NodePositions.Returns(nodePositions.ToFrozenDictionary());

        var project = ProjectFactory.CreateValidProject();
        string content = JsonSerializer.Serialize(project);

        var response = await _client.PostAsync(Endpoint, new StringContent(content, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task PVRPCloudRequest_InvalidInput_ReturnsBadRequest()
    {
        var project = ProjectFactory.CreateInvalidProject();
        string content = JsonSerializer.Serialize(project);

        var response = await _client.PostAsync(Endpoint, new StringContent(content, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PVRPCloudRequest_WrongJsonBody_ReturnsBadRequest()
    {
        var project = ProjectFactory.CreateInvalidProject();
        string content = JsonSerializer.Serialize(project);

        string newContent = content.Replace("{", "");

        var response = await _client.PostAsync(Endpoint, new StringContent(newContent, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PVRPCloudRequest_WrongJsonBodyWithNull_ReturnsBadRequest()
    {
        var project = ProjectFactory.CreateInvalidProject();
        string content = JsonSerializer.Serialize(project);

        string newContent = content.Replace("MaxTourDuration\":5", "MaxTourDuration:\":null");

        var response = await _client.PostAsync(Endpoint, new StringContent(newContent, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}