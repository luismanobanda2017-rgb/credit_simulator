using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class JwtTokenService(IConfiguration configuration)
{
	public (string Token, DateTime ExpiresAt) CreateToken(User user)
	{
		var jwt = configuration.GetSection("Jwt");
		var expiresAt = DateTime.UtcNow.AddHours(2);
		var key = jwt["Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
		};
		var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
		var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims, expires: expiresAt, signingCredentials: credentials);
		return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
	}
}
