using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Options;
using WebApi.Services;

namespace WebApi.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public sealed class UsersController(KeycloakService keycloakService, IOptions<KeycloakConfiguration> options) : ControllerBase
{
    [HttpGet]
    [Authorize("UserGetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users";
        var response = await keycloakService.GetAsync<List<UserDto>>(endpoint, true, cancellationToken);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    [Authorize("UserGetAll")]
    public async Task<IActionResult> GetByEmail(string email ,CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users?email={email}";
        var response = await keycloakService.GetAsync<List<UserDto>>(endpoint, true, cancellationToken);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    [Authorize("UserGetAll")]
    public async Task<IActionResult> GetByUserName(string username, CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users?username={username}";
        var response = await keycloakService.GetAsync<List<UserDto>>(endpoint, true, cancellationToken);
        return StatusCode(Response.StatusCode, response);
    }
    [HttpGet]
    [Authorize("UserGetAll")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{id}";
        var response = await keycloakService.GetAsync<UserDto>(endpoint, true, cancellationToken);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut]
    [Authorize("UserUpdate")]
    public async Task<IActionResult> Update(Guid id,UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{id}";

        var response = await keycloakService.PutAsync<string>(endpoint, request, true, cancellationToken);

        if (response.IsSuccessful && response.Data is null)
        {
            response.Data = "UserUpdate is successfull";
        }
        return StatusCode(response.StatusCode, response);

    }

    [HttpDelete]
    [Authorize("UserDelete")]
    public async Task<IActionResult> DeleteById(Guid id, CancellationToken cancellationToken = default)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{id}";

        var response = await keycloakService.DeleteAsync<string>(endpoint, true, cancellationToken);

        if (response.IsSuccessful && response.Data is null)
        {
            response.Data = "UserDelete is successfull";
        }
        return StatusCode(response.StatusCode, response);

    }
}
