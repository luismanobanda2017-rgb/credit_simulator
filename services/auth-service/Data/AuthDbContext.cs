using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
	public DbSet<User> Users => Set<User>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(entity =>
		{
			entity.ToTable("users");
			entity.HasKey(user => user.Id);
			entity.Property(user => user.Id).HasColumnName("id");
			entity.Property(user => user.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
			entity.Property(user => user.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
			entity.Property(user => user.PasswordHash).HasColumnName("password_hash").IsRequired();
			entity.Property(user => user.Status).HasColumnName("status").HasDefaultValue((short)1).IsRequired();
			entity.Property(user => user.CreatedAt).HasColumnName("created_at");
			entity.HasIndex(user => user.Username).IsUnique();
			entity.HasIndex(user => user.Email).IsUnique();
		});
	}
}
