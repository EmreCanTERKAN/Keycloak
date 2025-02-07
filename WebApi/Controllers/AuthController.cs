using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
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
        var stringData = JsonSerializer.Serialize(data);

        var content = new StringContent(stringData, Encoding.UTF8, "application/json");

        HttpClient httpClient = new();

        string token = await keycloakService.GetAccessToken();

        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var message = await httpClient.PostAsync(endpoint, content, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {

            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                return BadRequest(new { ErrorMessage =  errorResultForBadRequest!.ErrorMessage });
            }
            var errorResultForOther = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
            return BadRequest (new { ErrorMessage = errorResultForOther!.ErrorMessage });
        }

        return Ok(new { Message = "Kullanıcı oluşturuldu" });


    }
}
