namespace SimulationService.Dtos;

public class SimulationDto
{
	public int Id { get; init; }
	public Guid UserId { get; init; }
	public int CreditTypeId { get; init; }
	public string CreditTypeName { get; init; } = string.Empty;
	public decimal Amount { get; init; }
	public int Months { get; init; }
	public decimal AnnualRate { get; init; }
	public string Method { get; init; } = string.Empty;
	public decimal TotalInterest { get; init; }
	public decimal TotalPaid { get; init; }
	public DateTime CreatedAt { get; init; }
	public List<InstallmentDto> Installments { get; init; } = [];
}
