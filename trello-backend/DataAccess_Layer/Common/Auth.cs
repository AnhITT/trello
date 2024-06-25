using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace DataAccess_Layer.Common
{
    public class Auth
    {
        private readonly IHttpContextAccessor _contextAccessor;
        
        public Auth(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public string GetAccessToken()
        {
            var context = _contextAccessor.HttpContext;

            if (context != null && context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            return null;
        }
        public string GetUserIdCurrent()
        {
            var jwt = GetAccessToken();
            if (jwt == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var userIdClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Id");

            return userIdClaim?.Value;
        }
    }
}
