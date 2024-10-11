using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity.Data;
using PlaceApi.Identity.Tests.Integration.Common;

namespace PlaceApi.Identity.Tests.Integration.Endpoints.Register;

public class RegisterHttpClient : BaseHttpClient
{
    private const string RegisterEndpoint = "/api/v1/register";

    public RegisterHttpClient(HttpClient httpClient)
        : base(httpClient) { }

    public Task<HttpResponseMessage> RegisterAsync(RegisterRequest? registerRequest = null)
    {
        registerRequest ??= RegisterRequestFactory.CreateRegisterRequest();
        return PostAsJsonAsync(RegisterEndpoint, registerRequest);
    }

    public Task<HttpResponseMessage> RegisterWithInvalidEmailAsync()
    {
        RegisterRequest request = new() { Email = "invalid-email", Password = "ValidPassword123!" };
        return RegisterAsync(request);
    }

    public Task<HttpResponseMessage> RegisterWithWeakPasswordAsync()
    {
        RegisterRequest request = new() { Email = "user@example.com", Password = "weak" };
        return RegisterAsync(request);
    }

    public Task<HttpResponseMessage> RegisterWithExistingEmailAsync(string existingEmail)
    {
        RegisterRequest request = new() { Email = existingEmail, Password = "ValidPassword123!" };
        return RegisterAsync(request);
    }
}
