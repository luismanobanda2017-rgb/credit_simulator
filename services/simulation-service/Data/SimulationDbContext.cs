using Microsoft.EntityFrameworkCore;
using SimulationService.Models;

namespace SimulationService.Data;

public class SimulationDbContext(DbContextOptions<SimulationDbContext> options) : DbContext(options)
{
	public DbSet<Simulation> Simulations => Set<Simulation>();
	public DbSet<Installment> Installments => Set<Installment>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Simulation>(entity =>
		{
			entity.ToTable("simulations");
			entity.HasKey(item => item.Id);
			entity.Property(item => item.Id).HasColumnName("id");
			entity.Property(item => item.UserId).HasColumnName("user_id");
			entity.Property(item => item.CreditTypeId).HasColumnName("credit_type_id");
			entity.Property(item => item.CreditTypeName).HasColumnName("credit_type_name").HasMaxLength(100);
			entity.Property(item => item.Amount).HasColumnName("amount").HasPrecision(18, 2);
			entity.Property(item => item.Months).HasColumnName("months");
			entity.Property(item => item.AnnualRate).HasColumnName("annual_rate").HasPrecision(5, 2);
			entity.Property(item => item.Method).HasColumnName("method").HasMaxLength(20);
			entity.Property(item => item.TotalInterest).HasColumnName("total_interest").HasPrecision(18, 2);
			entity.Property(item => item.TotalPaid).HasColumnName("total_paid").HasPrecision(18, 2);
			entity.Property(item => item.CreatedAt).HasColumnName("created_at");
			entity.HasMany(item => item.Installments).WithOne(item => item.Simulation).HasForeignKey(item => item.SimulationId).OnDelete(DeleteBehavior.Cascade);
		});
		modelBuilder.Entity<Installment>(entity =>
		{
			entity.ToTable("installments");
			entity.HasKey(item => item.Id);
			entity.Property(item => item.Id).HasColumnName("id");
			entity.Property(item => item.SimulationId).HasColumnName("simulation_id");
			entity.Property(item => item.Number).HasColumnName("number");
			entity.Property(item => item.Payment).HasColumnName("payment").HasPrecision(18, 2);
			entity.Property(item => item.Interest).HasColumnName("interest").HasPrecision(18, 2);
			entity.Property(item => item.Principal).HasColumnName("principal").HasPrecision(18, 2);
			entity.Property(item => item.Balance).HasColumnName("balance").HasPrecision(18, 2);
		});
	}
}
