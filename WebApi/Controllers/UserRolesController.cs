using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Dtos;
using WebApi.Options;
using WebApi.Services;

namespace WebApi.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public sealed class UserRolesController(KeycloakService keycloakService, IOptions<KeycloakConfiguration> options) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AssignmentRolesByUserId(Guid id, List<RoleDto> request,CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{id}/role-mappings/clients/{options.Value.ClientUUID}";

        var response = await keycloakService.PostAsync<string>(endpoint, request, true, cancellationToken);

        if (response.IsSuccessful && response.Data is null)
        {
            response.Data = "Roles assignments is succesfull";
        }

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> UnAssignmentRolesByUserId(Guid id, List<RoleDto> request, CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{id}/role-mappings/clients/{options.Value.ClientUUID}";

        var response = await keycloakService.DeleteAsync<string>(endpoint, request, true, cancellationToken);

        if (response.IsSuccessful && response.Data is null)
        {
            response.Data = "Roles Unassignments is succesfull";
        }

        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUserRolesById(Guid id, CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users/{id}/role-mappings/clients/{options.Value.ClientUUID}";

        var response = await keycloakService.GetAsync<object>(endpoint, true, cancellationToken);

        return StatusCode(response.StatusCode, response);
    }
}
