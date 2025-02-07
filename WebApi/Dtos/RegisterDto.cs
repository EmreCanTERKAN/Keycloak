namespace WebApi.Dtos;

public sealed record RegisterDto(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string Password
    );

