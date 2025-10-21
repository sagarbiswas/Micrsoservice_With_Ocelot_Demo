using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;
        private readonly IConfiguration _configuration;

        public JwtTokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["JwtSettings:SecretKey"]?? Constants.SecretKey;
            _issuer = _configuration["JwtSettings:Issuer"] ?? Constants.Issuer;
            _audience = _configuration["JwtSettings:Audience"] ?? Constants.Audience;
            _expirationMinutes = string.IsNullOrEmpty(_configuration["JwtSettings:ExpirationMinutes"]) 
                ? Constants.ExpirationMinutes 
                : Convert.ToInt32(_configuration["JwtSettings:ExpirationMinutes"]);
        }
        
        /// <summary>
        /// This is called from client or ocelot to generate token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GenerateJwtToken(string username,string role= "least-privilege-user")
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(UserClaims.Username, username),
            new Claim(UserClaims.UserId, Guid.NewGuid().ToString()),
            new Claim(UserClaims.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
