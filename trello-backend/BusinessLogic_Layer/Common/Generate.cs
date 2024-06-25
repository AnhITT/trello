using DataAccess_Layer.DTOs;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic_Layer.Common
{
    public class Generate
    {
        private readonly JwtConfig _jwtConfig;
        public Generate(IOptions<JwtConfig> options)
        {
            _jwtConfig = options.Value;
        }
        public string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        public string CreateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var tokenChars = new char[8];
            for (int i = 0; i < tokenChars.Length; i++)
            {
                tokenChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(tokenChars);
        }
        public TokenEmail DecodeFromBase64(string base64String)
        {
            try
            {
                string decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
                string[] parts = decodedString.Split(':');
                if (parts.Length == 2)
                    return new TokenEmail
                    {
                        UserId = parts[0],
                        Token = parts[1]
                    };

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GenerateOTP()
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<string> GenerateJwtToken(User user)
        {
            var jwtTokenHanlder = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var claims = new ClaimsIdentity(new[]
            {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("Email", user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
            });
            //Token Description
            var tokenDescription = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Issuer = _jwtConfig.ValidIssuer,
                Audience = _jwtConfig.ValidAudience,
                Expires = DateTime.Now.Add(_jwtConfig.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = jwtTokenHanlder.CreateToken(tokenDescription);
            var jwtToken = jwtTokenHanlder.WriteToken(token);
            return jwtToken;
        }
    }
}