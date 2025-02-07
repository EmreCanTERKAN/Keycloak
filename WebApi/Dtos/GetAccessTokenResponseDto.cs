using System.Text.Json.Serialization;

namespace WebApi.Dtos;

public sealed class GetAccessTokenResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
}
