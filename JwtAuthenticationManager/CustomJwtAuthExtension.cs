using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JwtAuthenticationManager
{
    public static class CustomJwtAuthExtension
    {
        public static void AddCustomJwtAuthExtension(this IServiceCollection services, IConfiguration configuration)
        {
           /* Console.WriteLine("🔐 Loaded Configuration:");
            foreach (var kvp in configuration.AsEnumerable())
            {
                Console.WriteLine($"{kvp.Key} = {kvp.Value}");
            }*/

            var validIssuer = configuration["JwtSettings:Issuer"];
            var validAudience = configuration["JwtSettings:Audience"];
            var secretKey = configuration["JwtSettings:SecretKey"];
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }
    }
}
