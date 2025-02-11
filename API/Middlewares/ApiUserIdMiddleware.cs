using System.Security.Claims;

namespace API.Middlewares
{
    public class ApiUserIdMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var roles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                if (!string.IsNullOrEmpty(userId))
                {
                    context.Request.Headers["X-User-Id"] = userId;
                }

                if (roles.Count != 0)
                {
                    context.Request.Headers["X-User-Roles"] = string.Join(",", roles);
                }
            }

            await next(context);
        }
    }
}
