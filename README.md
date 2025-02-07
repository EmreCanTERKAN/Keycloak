# KeycloakService - .NET API için Kimlik Doğrulama Yönetimi

Bu proje, .NET tabanlı bir uygulama için **Keycloak** kimlik doğrulama işlemlerini yönetmek amacıyla geliştirilmiştir.

## 🚀 Özellikler
- **Access Token Alma**: Client Credentials Flow ile Keycloak üzerinden erişim token'ı alır.
- **Kullanıcı Yönetimi**: Keycloak API'lerini kullanarak kullanıcı oluşturma, güncelleme ve silme işlemleri yapılabilir.
- **Rol Yönetimi**: Kullanıcılara belirli roller atama işlemleri desteklenir.


## 📌 Kurulum
### 1️⃣ Gereksinimler
- .NET 9.0
- Keycloak Sunucusu (Docker ile çalıştırabilir veya harici bir sunucu kullanabilirsiniz.)

### 2️⃣ Projeyi Klonlayın
```bash
git clone https://github.com/kullaniciadi/KeycloakService.git
cd KeycloakService
```

### 3️⃣ Konfigürasyonları Güncelleyin
**appsettings.json** dosyanızda Keycloak bilgilerini ekleyin:
```json
{
  "KeycloakConfiguration": {
    "HostName": "http://localhost:8080",
    "ClientId": "myclient",
    "Realm": "myrealm",
    "ClientSecret": "c8SnrXJ1quMXfl4bbCefW1FTSK83idj5",
    "ClientUUID": "f20c0dc6-52d2-4ec8-bd5d-455b4d9059c9"
  }
}
```

### 4️⃣ Bağımlılıkları Yükleyin
```bash
dotnet restore
```

### 5️⃣ Uygulamayı Çalıştırın
```bash
dotnet run
```


**Örnek C# kodu ile erişim:**
```csharp
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

        var content = new FormUrlEncodedContent(data);
        var message = await client.PostAsync(endPoint, content, cancellationToken);
        var response = await message.Content.ReadAsStringAsync();


        if (!message.IsSuccessStatusCode)
        {

            if (message.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorResultForBadRequest = JsonSerializer.Deserialize<BadRequestErrorResponseDto>(response);
                throw new ArgumentException(errorResultForBadRequest?.ErrorMessage);
            }
            var errorResultForOther = JsonSerializer.Deserialize<ErrorResponseDto>(response);
            throw new ArgumentException("Bir şeyler ters gitti...");
        }

        var result = JsonSerializer.Deserialize<GetAccessTokenResponseDto>(response);
        return result!.AccessToken;

    }
```

## 📌 Bağımlılık Enjeksiyonu (DI)
**KeycloakService'i DI Konteynerine Kaydetme**
```csharp
builder.Services.AddScoped<KeycloakService>();
```

## 🛠 Keycloak Docker ile Çalıştırma
Eğer yerel bir Keycloak sunucusu başlatmak istiyorsanız, aşağıdaki Docker komutunu kullanabilirsiniz:
```bash
docker run -d -p 8080:8080 --name keycloak \
-e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin \
quay.io/keycloak/keycloak:latest start-dev
```

Bu işlem sonrası Keycloak’a **http://localhost:8080/** adresinden erişebilirsiniz.

## 📜 Lisans
Bu proje MIT lisansı ile lisanslanmıştır.


