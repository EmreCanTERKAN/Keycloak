using System.Text.Json.Serialization;

namespace WebApi.Dtos;

public class RoleDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
}
