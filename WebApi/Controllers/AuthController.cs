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

        return StatusCode(response.StatusCode, response);

    }
}
