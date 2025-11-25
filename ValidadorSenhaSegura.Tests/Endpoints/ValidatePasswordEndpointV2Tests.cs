using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

public class ValidatePasswordEndpointV2Tests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ValidatePasswordEndpointV2Tests(WebApplicationFactory<Program> factory)
    {
        var options = new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:7218/api")
        };

        _client = factory.CreateClient(options);
    }

    [Fact]
    public async Task ShouldReturn200ForValidPassword()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/validate-password");
        request.Headers.Add("api-version", "2.0");
        request.Content = JsonContent.Create(new { password = "Abc123!@#" });

        var response = await _client.SendAsync(request);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ShouldReturn400ForInvalidPassword()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/validate-password");
        request.Headers.Add("api-version", "2.0");
        request.Content = JsonContent.Create(new { password = "Abc12 !@#" });

        var response = await _client.SendAsync(request);

        Assert.False(response.IsSuccessStatusCode);
    }
}
