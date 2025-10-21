using Common.Models;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly JwtTokenHandler _JwtTokenHandler;
    public AuthController(IConfiguration configuration, JwtTokenHandler jwtTokenHandler)
    {
        _JwtTokenHandler = jwtTokenHandler;
        _configuration = configuration;
    }
    /// <summary>
    /// This method is using to create token.
    /// </summary>
    /// <param name="request"> Use Username  "demo" and Password "demo123" will work only for testing</param>
    /// <returns></returns>
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        // TODO: Add actual user validation logic
        if (request.Username == "nonuser" || string.IsNullOrEmpty(request.Password))
        {
            return Unauthorized();
        }
        var token = _JwtTokenHandler.GenerateJwtToken(request.Username, "least privilege user");
        return Ok(new LoginResponse
        {
            Token = token,
            Username = request.Username,
            Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(string.IsNullOrEmpty(_configuration["JwtSettings:ExpirationMinutes"]) 
            ? Constants.ExpirationMinutes 
            : Convert.ToInt32(_configuration["JwtSettings:ExpirationMinutes"])))
        });
       
    }
     /* // Moved to common JwtAuthenticaitonManager
    private string GenerateJwtToken(string username)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expirationMinutes = Convert.ToDouble(_configuration["JwtSettings:ExpirationMinutes"]);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(UserClaims.Username, username),
            new Claim(UserClaims.UserId, Guid.NewGuid().ToString()),
            new Claim(UserClaims.Role, "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
     */
}