using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Options;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public sealed class AuthController(IOptions<KeycloakConfiguration> options, KeycloakService keycloakService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users";
        object data = new
        {
            username = registerDto.UserName,
            firstName = registerDto.FirstName,
            lastName = registerDto.LastName,
            email = registerDto.Email,
            enabled = true,
            emailVerified = true,
            credentials = new List<object>
            {
                new
                {
                    type = "password",
                    temporary = false,
                    value = registerDto.Password
                }
            }
        };

        var response = await keycloakService.PostAsync<string>(endpoint, data, true, cancellationToken);

        if (response.IsSuccessful && response.Data is null)
        {
            response.Data = "UserCreate is successfull";
        }

        return StatusCode(response.StatusCode, response);

    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto request, CancellationToken cancellationToken = default)
    {
        string endPoint = $"{options.Value.HostName}/realms/{options.Value.Realm}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = new();
        KeyValuePair<string, string> grantType = new("grant_type", "password");
        KeyValuePair<string, string> clientId = new("client_id", options.Value.ClientId);
        KeyValuePair<string, string> clientSecret = new("client_secret", options.Value.ClientSecret);
        KeyValuePair<string, string> userName = new("username", request.UserName);
        KeyValuePair<string, string> password = new("password", request.Password);
        data.Add(grantType);
        data.Add(clientId);
        data.Add(clientSecret);
        data.Add(userName);
        data.Add(password);

        var response = await keycloakService.PostUrlEncodedFormAsync<object>(endPoint, data, false, cancellationToken);

        return StatusCode(response.StatusCode, response);

    }


}
