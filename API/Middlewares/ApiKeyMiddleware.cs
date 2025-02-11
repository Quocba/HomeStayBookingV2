using System.Text.Json;

namespace API.Middlewares
{
    public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly string _apiKeyHeaderName = "X-Api-Key";
        private readonly string _configuredApiKey = configuration["ApiKey"];

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(_apiKeyHeaderName, out var providedApiKey))
            {
                await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "API Key is missing.");
                return;
            }

            if (string.IsNullOrEmpty(_configuredApiKey) || !_configuredApiKey.Equals(providedApiKey))
            {
                await WriteErrorResponse(context, StatusCodes.Status403Forbidden, "Unauthorized client.");
                return;
            }

            await next(context);
        }

        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
        {
            var apiResponse = new
            {
                Success = false,
                StatusCode = statusCode,
                Message = MessageByStatusCode.GetMessageByStatusCode(statusCode),
                Data = message
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var response = JsonSerializer.Serialize(apiResponse);

            await context.Response.WriteAsync(response);
        }
    }
}
