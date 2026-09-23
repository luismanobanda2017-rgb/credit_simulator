namespace SimulationService.Models;

public class Simulation
{
	public int Id { get; set; }
	public Guid UserId { get; set; }
	public int CreditTypeId { get; set; }
	public string CreditTypeName { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public int Months { get; set; }
	public decimal AnnualRate { get; set; }
	public string Method { get; set; } = string.Empty;
	public decimal TotalInterest { get; set; }
	public decimal TotalPaid { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public List<Installment> Installments { get; set; } = [];
}
