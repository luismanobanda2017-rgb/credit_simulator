using LoginAuditService.Data;
using LoginAuditService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginAuditService.Controllers;

[ApiController]
[Route("api/login-attempts")]
public class LoginAttemptsController(LoginAuditDbContext db) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Register(LoginAttempt request, CancellationToken cancellationToken)
	{
		request.Id = 0;
		if (request.RegisteredAt == default)
			request.RegisteredAt = DateTime.UtcNow;
		db.LoginAttempts.Add(request);
		await db.SaveChangesAsync(cancellationToken);
		var recentAttempts = await db.LoginAttempts
			.AsNoTracking()
			.Where(attempt => attempt.Username == request.Username)
			.OrderByDescending(attempt => attempt.RegisteredAt)
			.ThenByDescending(attempt => attempt.Id)
			.ToListAsync(cancellationToken);
		var consecutiveFailures = recentAttempts.TakeWhile(attempt => !attempt.Successful).Count();
		var attemptNumber = request.Successful ? 1 : consecutiveFailures;
		request.AttemptNumber = attemptNumber;
		await db.SaveChangesAsync(cancellationToken);
		return Ok(new { request.Id, attemptNumber, consecutiveFailures });
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<LoginAttempt>>> Get(CancellationToken cancellationToken)
	{
		return Ok(await db.LoginAttempts.AsNoTracking().OrderByDescending(attempt => attempt.RegisteredAt).ToListAsync(cancellationToken));
	}
}