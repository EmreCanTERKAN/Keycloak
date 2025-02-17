using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Options;
using WebApi.Services;

namespace WebApi.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
// [Authorize]
public class RolesController(
    KeycloakService keycloakService,
    IOptions<KeycloakConfiguration> options): ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/clients/{options.Value.ClientUUID}/roles";
        var response = await keycloakService.GetAsync<List<RoleDto>>(endpoint, true, cancellationToken);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/clients/{options.Value.ClientUUID}/roles/{name}";
        var response = await keycloakService.GetAsync<RoleDto>(endpoint, true, cancellationToken);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateForClient(CreateRoleDto createRoleDto, CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/clients/{options.Value.ClientUUID}/roles";
        var response = await keycloakService.PostAsync<string>(endpoint, createRoleDto, true, cancellationToken);

        if (response.IsSuccessful && response.Data is null)
        {
            response.Data = "Role create is successful";
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteByName(string name, CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/clients/{options.Value.ClientUUID}/roles/{name}";
        var response = await keycloakService.DeleteAsync<string>(endpoint, true, cancellationToken);

        if (response.IsSuccessful && response.Data is null)
        {
            response.Data = "Role delete is successful";
        }
        return StatusCode(Response.StatusCode, response);

        
    }


}
