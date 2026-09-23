namespace SimulationService.Models;

public class Installment
{
	public int Id { get; set; }
	public int SimulationId { get; set; }
	public int Number { get; set; }
	public decimal Payment { get; set; }
	public decimal Interest { get; set; }
	public decimal Principal { get; set; }
	public decimal Balance { get; set; }
	public Simulation Simulation { get; set; } = null!;
}
