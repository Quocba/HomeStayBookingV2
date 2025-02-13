using BusinessObject.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace API.Middlewares
{
    public class ApiResponseMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                if (context.Response.ContentType?.StartsWith("application/vnd.openxmlformats-officedocument") == true)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                context.Response.Body = originalBodyStream;

                var statusCode = context.Response.StatusCode;
                var isSuccess = statusCode >= 200 && statusCode < 300;
              
                responseBody.Seek(0, SeekOrigin.Begin);
                var bodyContent = await new StreamReader(responseBody).ReadToEndAsync();

                var apiResponse = new
                {
                    Success = isSuccess,
                    StatusCode = statusCode,
                    Message = MessageByStatusCode.GetMessageByStatusCode(statusCode),
                    Data = isSuccess ? JsonSerializer.Deserialize<object>(bodyContent) : null
                };

                var response = JsonSerializer.Serialize(apiResponse);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(context, originalBodyStream, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (InvalidCredentialsException ex)
            {
                await HandleExceptionAsync(context, originalBodyStream, StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(context, originalBodyStream, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await HandleExceptionAsync(context, originalBodyStream, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, originalBodyStream, StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Stream originalBodyStream, int statusCode, string message)
        {
            context.Response.Body = originalBodyStream;

            var apiResponse = new
            {
                Success = false,
                StatusCode = statusCode,
                Message = MessageByStatusCode.GetMessageByStatusCode(statusCode),
                Data = message
            };

            var response = JsonSerializer.Serialize(apiResponse);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response);
        }
    }

}
