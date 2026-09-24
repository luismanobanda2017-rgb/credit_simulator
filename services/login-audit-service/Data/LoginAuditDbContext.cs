using LoginAuditService.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginAuditService.Data;

public class LoginAuditDbContext(DbContextOptions<LoginAuditDbContext> options) : DbContext(options)
{
	public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<LoginAttempt>(entity =>
		{
			entity.ToTable("login_attempts");
			entity.HasKey(attempt => attempt.Id);
			entity.Property(attempt => attempt.Id).HasColumnName("id");
			entity.Property(attempt => attempt.UserId).HasColumnName("user_id");
			entity.Property(attempt => attempt.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
			entity.Property(attempt => attempt.RegisteredAt).HasColumnName("registered_at");
			entity.Property(attempt => attempt.Successful).HasColumnName("successful").IsRequired();
			entity.Property(attempt => attempt.AttemptNumber).HasColumnName("attempt_number").IsRequired();
		});
	}
}