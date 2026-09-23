using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthDbContext db, JwtTokenService tokenService) : ControllerBase
{
	[HttpPost("register")]
	public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
	{
		var username = request.Username.Trim();
		var email = request.Email.Trim().ToLowerInvariant();
		if (await db.Users.AnyAsync(user => user.Username == username || user.Email == email, cancellationToken))
			return Conflict(new { message = "El usuario o correo ya está registrado." });

		var user = new User { Username = username, Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password) };
		db.Users.Add(user);
		await db.SaveChangesAsync(cancellationToken);
		var result = tokenService.CreateToken(user);
		return Ok(new AuthResponse(result.Token, user.Username, result.ExpiresAt));
	}

	[HttpPost("login")]
	public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
	{
		var user = await db.Users.SingleOrDefaultAsync(candidate => candidate.Username == request.Username.Trim(), cancellationToken);
		if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
			return Unauthorized(new { message = "Usuario o contraseña inválidos." });

		var result = tokenService.CreateToken(user);
		return Ok(new AuthResponse(result.Token, user.Username, result.ExpiresAt));
	}
}
