using System.Collections.Frozen;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using GMap.NET;
using NSubstitute;
using PMapCore.BO;
using PMapCore.Route;

namespace PVRPCloudApiTests;

public class PVRPCloudRequestTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private const string Endpoint = "/v1/PVRPCloudRequest";

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PVRPCloudRequest_ValidInput_MakesAValidResponse()
    {
        var project = ProjectFactory.CreateValidProject();

        factory.PVRPCloudLogic.Handle(project).Returns("12345678");

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