using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_Layer.Utilities
{
    public class AuthContext
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthContext()
        {
            _contextAccessor = new HttpContextAccessor();
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