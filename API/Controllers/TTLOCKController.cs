using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using BusinessObject.Interfaces;
using BusinessObject.Entities;
using BusinessObject.Shares;
using Microsoft.EntityFrameworkCore;
using BusinessObject.DTO;

[ApiController]
[Route("api/ttlock")]
public class TTLOCKController(HttpClient _client, IConfiguration _configuration, IRepository<TTlockAccuont> _ttlockRepository) : ControllerBase
{

        [HttpPost("add-ttlock-account")]
        public async Task<IActionResult> AddTTLockAccount([FromBody]TTlockAccuontDTO request)
        {
            var addTTLockAccount = new TTlockAccuont
            {
                TTLockUserName = request.TTLockUserName,
                Password = Util.GenerateMD5(request.Password)!,
                TTLockID = Guid.NewGuid(),
                HomeStayID = request.HomeStayID
            };

            await _ttlockRepository.AddAsync(addTTLockAccount);
            await _ttlockRepository.SaveAsync();

            return Ok(new { Message = "Add TTLockAccount Success" });
        }

    [HttpPost("user-locks")]
    public async Task<IActionResult> GetUserLocks([FromQuery] Guid homeStayID)
    {
        var ttlockAccount = await _ttlockRepository.FindWithInclude()
                           .Include(x => x.HomeStay)
                           .FirstOrDefaultAsync(x => x.HomeStayID == homeStayID);

        if (ttlockAccount == null)
            return NotFound(new { message = "TTLock account not found for this homestay." });

        var clientId = _configuration["TTLOCKConfig:client_id"];
        var clientSecret = _configuration["TTLOCKConfig:client_secret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            return StatusCode(500, "TTLock client_id or client_secret is not configured.");

        var tokenForm = new Dictionary<string, string>
        {
            ["clientId"] = clientId,
            ["clientSecret"] = clientSecret,
            ["username"] = ttlockAccount.TTLockUserName!,
            ["password"] = ttlockAccount.Password!,
            ["grant_type"] = "password"
        };

        var tokenResponse = await _client.PostAsync("https://euapi.ttlock.com/oauth2/token", new FormUrlEncodedContent(tokenForm));
        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)tokenResponse.StatusCode, new
            {
                message = "Error getting access_token",
                response = tokenJson
            });
        }

        var tokenObj = JsonSerializer.Deserialize<JsonElement>(tokenJson);
        if (!tokenObj.TryGetProperty("access_token", out var accessTokenProp))
        {
            return BadRequest(new
            {
                message = "Access_token not found",
                response = tokenJson
            });
        }

        var accessToken = accessTokenProp.GetString();

        //List lock of account
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var url = $"https://euapi.ttlock.com/v3/lock/list?" +
                  $"clientId={clientId}&" +
                  $"accessToken={accessToken}&" +
                  $"pageNo={1}&" +
                  $"pageSize={int.MaxValue}&" +
                  $"date={timestamp}";

        var lockResponse = await _client.GetAsync(url);
        var lockJson = await lockResponse.Content.ReadAsStringAsync();

        if (lockResponse.IsSuccessStatusCode)
        {
            var lockData = JsonSerializer.Deserialize<JsonElement>(lockJson);

            if (lockData.TryGetProperty("list", out var lockList))
            {
                var result = new List<GetTTLockResponse>();

                foreach (var item in lockList.EnumerateArray())
                {
                    var dto = new GetTTLockResponse
                    {
                        lockId = item.GetProperty("lockId").GetInt32(),
                        lockAlias = item.GetProperty("lockAlias").GetString(),
                        electricQuantity = item.GetProperty("electricQuantity").GetInt32()
                    };

                    result.Add(dto);
                }

                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "Không tìm thấy danh sách khóa trong phản hồi." });
            }
        }
        return StatusCode((int)lockResponse.StatusCode, new
        {
            message = "Lỗi khi lấy danh sách khóa",
            response = lockJson
        });
    }


    [HttpPost("keyboard-passwords")]
    public async Task<IActionResult> GetKeyboardPasswords([FromForm]Guid homeStayID,[FromForm] int lockID)
    {
        try
        {
            var ttlockAccount = await _ttlockRepository.FindWithInclude()
                               .Include(x => x.HomeStay)
                               .FirstOrDefaultAsync(x => x.HomeStayID == homeStayID);

            if (ttlockAccount == null)
                return NotFound(new { message = "TTLock account not found for this homestay." });

            var clientId = _configuration["TTLOCKConfig:client_id"];
            var clientSecret = _configuration["TTLOCKConfig:client_secret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                return StatusCode(500, "TTLock client_id or client_secret is not configured.");

            var tokenForm = new Dictionary<string, string>
            {
                ["clientId"] = clientId,
                ["clientSecret"] = clientSecret,
                ["username"] = ttlockAccount.TTLockUserName!,
                ["password"] = ttlockAccount.Password!,
                ["grant_type"] = "password"
            };

            var tokenResponse = await _client.PostAsync("https://euapi.ttlock.com/oauth2/token", new FormUrlEncodedContent(tokenForm));
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)tokenResponse.StatusCode, new
                {
                    message = "Error getting access_token",
                    response = tokenJson
                });
            }

            var tokenObj = JsonSerializer.Deserialize<JsonElement>(tokenJson);
            if (!tokenObj.TryGetProperty("access_token", out var accessTokenProp))
            {
                return BadRequest(new
                {
                    message = "Access_token not found",
                    response = tokenJson
                });
            }

            var accessToken = accessTokenProp.GetString();
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int orderBy = 1;

            string url = $"https://euapi.ttlock.com/v3/lock/listKeyboardPwd" +
                         $"?clientId={clientId}" +
                         $"&accessToken={accessToken}" +
                         $"&lockId={lockID}" +
                         $"&pageNo={1}" +
                         $"&pageSize={int.MaxValue}" +
                         $"&orderBy={orderBy}" +
                         $"&date={timestamp}";

            var response = await _client.GetAsync(url);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = JsonSerializer.Deserialize<JsonElement>(responseContent);

                if (json.TryGetProperty("list", out var lockList))
                {
                    var result = new List<LockInfroDTO>();

                    foreach (var item in lockList.EnumerateArray())
                    {
                        var dto = new LockInfroDTO
                        {
                            LockID = item.GetProperty("lockId").GetInt32(),
                            LockPassword = long.TryParse(item.GetProperty("keyboardPwd").GetString(), out var pwd) ? pwd : 0,
                            LockName = item.GetProperty("keyboardPwdName").GetString()
                        };

                        result.Add(dto);
                    }

                    return Ok(result);
                }

                return BadRequest(new { message = "Key list not found in response." });
            }
            else
            {
                return StatusCode((int)response.StatusCode, new
                {
                    message = "TTLock API call failed.",
                    response = responseContent
                });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error when calling TTLock API", error = ex.Message });
        }
    }

        [HttpPost("lock-remote")]
        public async Task<IActionResult> LockRemote(Guid homeStayID, int lockID)
        {
            try
            {
                var ttlockAccount = await _ttlockRepository.FindWithInclude()
                    .Include(x => x.HomeStay)
                    .FirstOrDefaultAsync(x => x.HomeStayID == homeStayID);

                if (ttlockAccount == null)
                    return NotFound(new { message = "TTLock account not found for this homestay." });

                var clientId = _configuration["TTLOCKConfig:client_id"];
                var clientSecret = _configuration["TTLOCKConfig:client_secret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                    return StatusCode(500, "TTLock client_id or client_secret is not configured.");

                var tokenForm = new Dictionary<string, string>
                {
                    ["clientId"] = clientId,
                    ["clientSecret"] = clientSecret,
                    ["username"] = ttlockAccount.TTLockUserName!,
                    ["password"] = ttlockAccount.Password!,
                    ["grant_type"] = "password"
                };

                var tokenResponse = await _client.PostAsync("https://euapi.ttlock.com/oauth2/token", new FormUrlEncodedContent(tokenForm));
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)tokenResponse.StatusCode, new
                    {
                        message = "Error getting access_token",
                        response = tokenJson
                    });
                }

                var tokenObj = JsonSerializer.Deserialize<JsonElement>(tokenJson);
                if (!tokenObj.TryGetProperty("access_token", out var accessTokenProp))
                {
                    return BadRequest(new
                    {
                        message = "Access_token not found",
                        response = tokenJson
                    });
                }

                var accessToken = accessTokenProp.GetString();
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var requestData = new Dictionary<string, string>
                {
                    ["clientId"] = clientId,
                    ["accessToken"] = accessToken!,
                    ["lockId"] = lockID.ToString(),
                    ["date"] = timestamp.ToString()
                };

                var response = await _client.PostAsync("https://euapi.ttlock.com/v3/lock/lock", new FormUrlEncodedContent(requestData));
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new
                    {
                        message = "The lock was successfully closed!",
                        response = responseString
                    });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        message = "API lock call failed",
                        response = responseString
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when calling TTLock API", error = ex.Message });
            }
        }

    [HttpPost("unlock-remote")]
    public async Task<IActionResult> UnlockRemote(Guid homeStayID, int lockID)
    {
        try
        {

            var ttlockAccount = await _ttlockRepository.FindWithInclude()
                .Include(x => x.HomeStay)
                .FirstOrDefaultAsync(x => x.HomeStayID == homeStayID);

            if (ttlockAccount == null)
                return NotFound(new { message = "TTLock account not found for this homestay." });

            var clientId = _configuration["TTLOCKConfig:client_id"];
            var clientSecret = _configuration["TTLOCKConfig:client_secret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                return StatusCode(500, "TTLock client_id or client_secret is not configured.");

            var tokenForm = new Dictionary<string, string>
            {
                ["clientId"] = clientId,
                ["clientSecret"] = clientSecret,
                ["username"] = ttlockAccount.TTLockUserName!,
                ["password"] = ttlockAccount.Password!,
                ["grant_type"] = "password"
            };

            var tokenResponse = await _client.PostAsync("https://euapi.ttlock.com/oauth2/token", new FormUrlEncodedContent(tokenForm));
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)tokenResponse.StatusCode, new
                {
                    message = "Error getting access_token",
                    response = tokenJson
                });
            }

            var tokenObj = JsonSerializer.Deserialize<JsonElement>(tokenJson);
            if (!tokenObj.TryGetProperty("access_token", out var accessTokenProp))
            {
                return BadRequest(new
                {
                    message = "Access_token not found in response",
                    response = tokenJson
                });
            }

            var accessToken = accessTokenProp.GetString();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Gửi request mở khóa
            var unlockForm = new Dictionary<string, string>
            {
                ["clientId"] = clientId,
                ["accessToken"] = accessToken!,
                ["lockId"] = lockID.ToString(),
                ["date"] = timestamp.ToString()
            };

            var unlockResponse = await _client.PostAsync("https://euapi.ttlock.com/v3/lock/unlock", new FormUrlEncodedContent(unlockForm));
            var unlockJson = await unlockResponse.Content.ReadAsStringAsync();

            if (unlockResponse.IsSuccessStatusCode)
            {
                return Ok(new
                {
                    message = "Unlock command sent successfully!",
                    response = unlockJson
                });
            }
            else
            {
                return StatusCode((int)unlockResponse.StatusCode, new
                {
                    message = "Unlock API call failed!",
                    response = unlockJson
                });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = "Unknown error while unlocking",
                error = ex.Message
            });
        }
    }

    [HttpPost("change-keyboard-password")]
    public async Task<IActionResult> ChangeKeyboardPassword(Guid homeStayID, int lockID, long keyboardPwdId, string newPasscode, long startDate, long endDate, string? name = null)
    {
        try
        {
            // Lấy thông tin tài khoản TTLock theo HomeStay
            var ttlockAccount = await _ttlockRepository.FindWithInclude()
                .Include(x => x.HomeStay)
                .FirstOrDefaultAsync(x => x.HomeStayID == homeStayID);

            if (ttlockAccount == null)
                return NotFound(new { message = "TTLock account not found for this homestay." });

            var clientId = _configuration["TTLOCKConfig:client_id"];
            var clientSecret = _configuration["TTLOCKConfig:client_secret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                return StatusCode(500, "TTLock client_id or client_secret is not configured.");

            // Lấy access token
            var tokenForm = new Dictionary<string, string>
            {
                ["clientId"] = clientId,
                ["clientSecret"] = clientSecret,
                ["username"] = ttlockAccount.TTLockUserName!,
                ["password"] = ttlockAccount.Password!,
                ["grant_type"] = "password"
            };

            var tokenResponse = await _client.PostAsync("https://euapi.ttlock.com/oauth2/token", new FormUrlEncodedContent(tokenForm));
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

            if (!tokenResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)tokenResponse.StatusCode, new
                {
                    message = "Error getting access_token",
                    response = tokenJson
                });
            }

            var tokenObj = JsonSerializer.Deserialize<JsonElement>(tokenJson);
            if (!tokenObj.TryGetProperty("access_token", out var accessTokenProp))
            {
                return BadRequest(new
                {
                    message = "Access_token not found in response",
                    response = tokenJson
                });
            }

            var accessToken = accessTokenProp.GetString();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Gửi request đổi passcode
            var changeForm = new Dictionary<string, string>
            {
                ["clientId"] = clientId,
                ["accessToken"] = accessToken!,
                ["lockId"] = lockID.ToString(),
                ["keyboardPwdId"] = keyboardPwdId.ToString(),
                ["newKeyboardPwd"] = newPasscode,
                ["startDate"] = startDate.ToString(),
                ["endDate"] = endDate.ToString(),
                ["changeType"] = "2", // sử dụng API qua gateway hoặc WiFi
                ["date"] = timestamp.ToString()
            };

            // Tùy chọn thêm tên nếu có
            if (!string.IsNullOrEmpty(name))
            {
                changeForm["keyboardPwdName"] = name;
            }

            var changeResponse = await _client.PostAsync("https://euapi.ttlock.com/v3/keyboardPwd/change", new FormUrlEncodedContent(changeForm));
            var changeJson = await changeResponse.Content.ReadAsStringAsync();

            if (changeResponse.IsSuccessStatusCode)
            {
                return Ok(new
                {
                    message = "Password change command sent successfully!",
                    response = changeJson
                });
            }
            else
            {
                return StatusCode((int)changeResponse.StatusCode, new
                {
                    message = "API call to change passcode failed!",
                    response = changeJson
                });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = "Unknown error when changing passcode",
                error = ex.Message
            });
        }
    }



}
