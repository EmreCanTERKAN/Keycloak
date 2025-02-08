using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using TS.Result;
using WebApi.Dtos;
using WebApi.Options;


namespace WebApi.Services;

public sealed class KeycloakService(
    IOptions<KeycloakConfiguration> options)
{
    public async Task<string> GetAccessToken(CancellationToken cancellationToken = default)
    {
        HttpClient client = new();
        string endPoint = $"{options.Value.HostName}/realms/{options.Value.Realm}/protocol/openid-connect/token";

        List<KeyValuePair<string, string>> data = new();
        KeyValuePair<string, string> grantType = new("grant_type", "client_credentials");
        KeyValuePair<string, string> clientId = new("client_id", options.Value.ClientId);
        KeyValuePair<string, string> clientSecret = new("client_secret", options.Value.ClientSecret);
        data.Add(grantType);
        data.Add(clientId);
        data.Add(clientSecret);

        Result<GetAccessTokenResponseDto> result = await PostUrlEncodedFormAsync<GetAccessTokenResponseDto>(endPoint, data, false, cancellationToken);

        return result.Data!.AccessToken;

    }

    public async Task<Result<T>> PostAsync<T>(string endpoint, object data, bool requiredToken, CancellationToken cancellationToken)
    {
        var stringData = JsonSerializer.Serialize(data);

        var content = new StringContent(stringData, Encoding.UTF8, "application/json");

        HttpClient httpClient = new();

        if (requiredToken)
        {
        string token = await GetAccessToken();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }


        var message = await httpClient.PostAsync(endpoint, content, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();



        if (!message.IsSuccessStatusCode)
        {
            var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
            return Result<T>.Failure(errorResultForBadRequest?.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.Created)
        {
            return Result<T>.Succeed(default!);
        }
        var obj = JsonSerializer.Deserialize<T>(response);

        return Result<T>.Succeed(obj!);
    }

    public async Task<Result<T>> PostUrlEncodedFormAsync<T>(string endpoint, List<KeyValuePair<string,string>> data, bool requiredToken, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = new();

        if (requiredToken)
        {
            string token = await GetAccessToken();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }


        var message = await httpClient.PostAsync(endpoint, new FormUrlEncodedContent(data), cancellationToken);

        var response = await message.Content.ReadAsStringAsync();



        if (!message.IsSuccessStatusCode)
        {
            var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
            return Result<T>.Failure(errorResultForBadRequest?.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.Created)
        {
            return Result<T>.Succeed(default!);
        }
        var obj = JsonSerializer.Deserialize<T>(response);

        return Result<T>.Succeed(obj!);
    }
}
