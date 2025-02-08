namespace WebApi.Dtos;

public sealed record RegisterDto(
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string Password
    );

