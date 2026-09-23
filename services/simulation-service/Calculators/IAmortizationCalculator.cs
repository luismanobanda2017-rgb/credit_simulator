using SimulationService.Models;

namespace SimulationService.Calculators;

public interface IAmortizationCalculator
{
	List<Installment> Calculate(decimal amount, decimal annualRate, int months);
}
