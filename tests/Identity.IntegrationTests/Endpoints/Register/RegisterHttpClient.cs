using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Identity.IntegrationTests.Common;
using Identity.IntegrationTests.Endpoints.Register.V1;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.IntegrationTests.Endpoints.Register;

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
