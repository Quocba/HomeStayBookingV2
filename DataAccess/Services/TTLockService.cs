using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObject.Entities;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Services
{
    public class TTLockService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public TTLockService(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, string? AccessToken, string? ErrorMessage)> GetTTLockAccessTokenAsync(TTlockAccuont account)
        {
            var clientId = _configuration["TTLOCKConfig:client_id"];
            var clientSecret = _configuration["TTLOCKConfig:client_secret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                return (false, null, "TTLock client_id hoặc client_secret chưa được cấu hình.");

            var tokenForm = new Dictionary<string, string>
            {
                ["clientId"] = clientId,
                ["clientSecret"] = clientSecret,
                ["username"] = account.TTLockUserName!,
                ["password"] = account.Password!,
                ["grant_type"] = "password"
            };

            var tokenResponse = await _client.PostAsync("https://euapi.ttlock.com/oauth2/token", new FormUrlEncodedContent(tokenForm));
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
                return (false, null, $"Lỗi khi lấy access_token: {tokenJson}");

            var tokenObj = JsonSerializer.Deserialize<JsonElement>(tokenJson);
            if (!tokenObj.TryGetProperty("access_token", out var accessTokenProp))
                return (false, null, $"Không tìm thấy access_token trong response: {tokenJson}");

            var accessToken = accessTokenProp.GetString();
            return (true, accessToken, null);
        }
    }
}
