using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
}
