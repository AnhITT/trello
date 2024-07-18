using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DataAccess_Layer.Helpers
{
    public static class HttpContextHelpers
    {
        public static Guid GetCurrentUserId(this HttpContext httpContext)
        {
            var currentUserId = httpContext?.User.FindFirstValue("Id");
            Guid.TryParse(currentUserId, out var userId);
            return userId;
        }
    }
}
