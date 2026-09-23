using System.ComponentModel.DataAnnotations;

namespace SimulationService.Dtos;

public class SimulationRequest
{
	[Range(1, int.MaxValue)]
	public int CreditTypeId { get; set; }

	[Range(typeof(decimal), "0.01", "1000000000")]
	public decimal Amount { get; set; }

	[Range(1, 1200)]
	public int Months { get; set; }

	[Required]
	public string Method { get; set; } = string.Empty;
}
