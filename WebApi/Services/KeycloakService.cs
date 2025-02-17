using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using TS.Result;
using WebApi.Dtos;
using WebApi.Options;


namespace WebApi.Services;

public sealed class KeycloakService(
    IOptions<KeycloakConfiguration> options, HttpClient _httpClient)
{
    public async Task<string> GetAccessToken(CancellationToken cancellationToken = default)
    {
        
        string endPoint = $"{options.Value.HostName}/realms/{options.Value.Realm}/protocol/openid-connect/token";

        //List<KeyValuePair<string, string>> data = new();
        //KeyValuePair<string, string> grantType = new("grant_type", "client_credentials");
        //KeyValuePair<string, string> clientId = new("client_id", options.Value.ClientId);
        //KeyValuePair<string, string> clientSecret = new("client_secret", options.Value.ClientSecret);
        //data.Add(grantType);
        //data.Add(clientId);
        //data.Add(clientSecret);

        List<KeyValuePair<string, string>> data = new()
        {
            new ("grant_type","client_credentials"),
            new ("client_id",options.Value.ClientId),
            new ("client_secret",options.Value.ClientSecret),
        };

        Result<GetAccessTokenResponseDto> result = await PostUrlEncodedFormAsync<GetAccessTokenResponseDto>(endPoint, data, false, cancellationToken);

        return result.Data!.AccessToken;

    }

    public async Task<Result<T>> PostAsync<T>(string endpoint, object data, bool requiredToken, CancellationToken cancellationToken)
    {
        var stringData = JsonSerializer.Serialize(data);
        var content = new StringContent(stringData, Encoding.UTF8, "application/json");


        if (requiredToken)
        {
            string token = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        var response = await message.Content.ReadAsStringAsync();

        // Yanıt boşsa hata döndür
        if (string.IsNullOrWhiteSpace(response))
        {
            return Result<T>.Succeed(default!);
        }

        // JSON format kontrolü
        if (!response.StartsWith("{") && !response.StartsWith("["))
        {
            return Result<T>.Failure("Invalid JSON response");
        }

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                return Result<T>.Failure(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = JsonSerializer.Deserialize<ErrorResponseDto>(response);
            return Result<T>.Failure(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created)
        {
            return Result<T>.Succeed(default!);
        }

        // T türü string ise direkt döndür
        var obj = typeof(T) == typeof(string) ? (T)(object)response : JsonSerializer.Deserialize<T>(response);

        return Result<T>.Succeed(obj!);
    }


    public async Task<Result<T>> PostUrlEncodedFormAsync<T>(string endpoint, List<KeyValuePair<string, string>> data, bool requiredToken, CancellationToken cancellationToken = default)
    {

        if (requiredToken)
        {
            string token = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }


        var message = await _httpClient.PostAsync(endpoint, new FormUrlEncodedContent(data), cancellationToken);

        var response = await message.Content.ReadAsStringAsync();



        if (!message.IsSuccessStatusCode)
        {

            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                return Result<T>.Failure(errorResultForBadRequest!.ErrorDescription);
            }
            else if (message.StatusCode == HttpStatusCode.Unauthorized)
            {
                var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                return Result<T>.Failure(errorResultForBadRequest!.ErrorDescription);
            }


            var errorResultForOther = JsonSerializer.Deserialize<ErrorResponseDto>(response);
            return Result<T>.Failure(errorResultForOther!.ErrorMessage);

        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.Created)
        {
            return Result<T>.Succeed(default!);
        }
        var obj = JsonSerializer.Deserialize<T>(response);

        return Result<T>.Succeed(obj!);
    }

    public async Task<Result<T>> GetAsync<T>(string endpoint, bool requiredToken = false, CancellationToken cancellationToken = default)
    {


        if (requiredToken)
        {
            string token = await GetAccessToken();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }


        var message = await _httpClient.GetAsync(endpoint, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();



        if (!message.IsSuccessStatusCode)
        {

            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                return Result<T>.Failure(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = JsonSerializer.Deserialize<ErrorResponseDto>(response);
            return Result<T>.Failure(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.Created)
        {
            return Result<T>.Succeed(default!);
        }
        var obj = JsonSerializer.Deserialize<T>(response);

        return Result<T>.Succeed(obj!);
    }

    public async Task<Result<T>> PutAsync<T>(string endpoint, object data, bool reqToken = false, CancellationToken cancellationToken = default)
    {
        string stringData = JsonSerializer.Serialize(data);
        var content = new StringContent(stringData, Encoding.UTF8, "application/json");


        if (reqToken)
        {
            string token = await GetAccessToken();

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await _httpClient.PutAsync(endpoint, content, cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                return Result<T>.Failure(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = JsonSerializer.Deserialize<ErrorResponseDto>(response);
            return Result<T>.Failure(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.NoContent)
        {
            return Result<T>.Succeed(default!);
        }

        var obj = JsonSerializer.Deserialize<T>(response);

        return Result<T>.Succeed(obj!);
    }

    public async Task<Result<T>> DeleteAsync<T>(string endpoint, bool reqToken = false, CancellationToken cancellationToken = default)
    {

        if (reqToken)
        {
            string token = await GetAccessToken();

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var message = await _httpClient.DeleteAsync(endpoint,cancellationToken);

        var response = await message.Content.ReadAsStringAsync();

        if (!message.IsSuccessStatusCode)
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                return Result<T>.Failure(errorResultForBadRequest!.ErrorDescription);
            }

            var errorResultForOther = JsonSerializer.Deserialize<ErrorResponseDto>(response);
            return Result<T>.Failure(errorResultForOther!.ErrorMessage);
        }

        if (message.StatusCode == HttpStatusCode.Created || message.StatusCode == HttpStatusCode.NoContent)
        {
            return Result<T>.Succeed(default!);
        }

        var obj = JsonSerializer.Deserialize<T>(response);

        return Result<T>.Succeed(obj!);
    }
}
