using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthDbContext db, JwtTokenService tokenService, LoginAuditClient auditClient) : ControllerBase
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
		var username = request.Username.Trim();
		var user = await db.Users.SingleOrDefaultAsync(candidate => candidate.Username == username, cancellationToken);
		if (user is null)
		{
			await auditClient.RegisterAsync(username, null, false, cancellationToken);
			return Unauthorized(new { message = "Usuario o contraseña inválidos." });
		}

		if (user.Status == 0)
		{
			await auditClient.RegisterAsync(user.Username, user.Id, false, cancellationToken);
			return StatusCode(StatusCodes.Status423Locked, new { message = "El usuario está bloqueado por demasiados intentos fallidos." });
		}

		if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
		{
			var audit = await auditClient.RegisterAsync(user.Username, user.Id, false, cancellationToken);
			if (audit.ConsecutiveFailures >= 3)
			{
				user.Status = 0;
			}
			await db.SaveChangesAsync(cancellationToken);
			if (user.Status == 0)
				return StatusCode(StatusCodes.Status423Locked, new { message = "Usuario bloqueado después de 3 intentos fallidos." });
			return Unauthorized(new { message = $"Usuario o contraseña inválidos. Intento fallido {audit.AttemptNumber} de 3." });
		}

		await db.SaveChangesAsync(cancellationToken);
		await auditClient.RegisterAsync(user.Username, user.Id, true, cancellationToken);
		var result = tokenService.CreateToken(user);
		return Ok(new AuthResponse(result.Token, user.Username, result.ExpiresAt));
	}
}
