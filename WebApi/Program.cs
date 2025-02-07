using Scalar.AspNetCore;
using WebApi.Options;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddOpenApi();


builder.Services.Configure<KeycloakConfiguration>(builder.Configuration.GetSection("KeycloakConfiguration"));
builder.Services.AddScoped<KeycloakService>();

builder.Services.AddControllers();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseCors(x =>
{
    x.AllowAnyHeader();
    x.AllowAnyMethod();
    x.AllowAnyOrigin();
});
app.MapOpenApi();
app.MapScalarApiReference();



app.MapGet("/get-access-token", async (KeycloakService keycloakService) =>
{
    var token = await keycloakService.GetAccessToken(default);
    return Results.Ok(new { AccessToken = token});
});

app.MapControllers();

app.Run();
