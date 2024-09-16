using System.Net;
using Azure;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PVRPCloud.Models;
using PVRPCloudApi.Handlers;

namespace PVRPCloudApiTests;

public class PVRPCloudResultTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private const string Endpoint = "/v1/PVRPCloudResult/";

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PVRPCloudResult_ExistingId_ReturnsOk()
    {
        factory.QueueResponseHandler.Handle(Arg.Any<string>()).Returns(new ProjectRes());

        string requestId = "12345678";

        var response = await _client.GetAsync(Endpoint + requestId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PVRPCloudResult_NonExistingId_ReturnsNotFound()
    {
        factory.QueueResponseHandler.Handle(Arg.Any<string>()).ThrowsAsync(new RequestFailedException(status: 404, message: ""));

        string requestId = "1234567";

        var response = await _client.GetAsync(Endpoint + requestId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
