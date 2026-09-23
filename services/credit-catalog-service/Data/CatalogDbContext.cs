using CreditCatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditCatalogService.Data;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
	public DbSet<CreditType> CreditTypes => Set<CreditType>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<CreditType>(entity =>
		{
			entity.ToTable("credit_types");
			entity.HasKey(type => type.Id);
			entity.Property(type => type.Id).HasColumnName("id");
			entity.Property(type => type.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
			entity.Property(type => type.AnnualRate).HasColumnName("annual_rate").HasPrecision(5, 2);
			entity.Property(type => type.MinAmount).HasColumnName("min_amount").HasPrecision(18, 2);
			entity.Property(type => type.MaxAmount).HasColumnName("max_amount").HasPrecision(18, 2);
			entity.Property(type => type.MaxMonths).HasColumnName("max_months");
			entity.Property(type => type.IsActive).HasColumnName("is_active");
		});
	}
}
