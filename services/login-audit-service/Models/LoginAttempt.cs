namespace LoginAuditService.Models;

public class LoginAttempt
{
	public long Id { get; set; }
	public Guid? UserId { get; set; }
	public string Username { get; set; } = string.Empty;
	public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
	public bool Successful { get; set; }
	public int AttemptNumber { get; set; }
}