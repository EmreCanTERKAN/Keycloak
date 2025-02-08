﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TS.Result;
using WebApi.Dtos;
using WebApi.Options;
using WebApi.Services;

namespace WebApi.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public sealed class UsersController(KeycloakService keycloakService, IOptions<KeycloakConfiguration> options) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        string endpoint = $"{options.Value.HostName}/admin/realms/{options.Value.Realm}/users";
        var response = await keycloakService.GetAsync<List<UserDto>>(endpoint, true, cancellationToken);
        return StatusCode(Response.StatusCode, response);
    }
}
