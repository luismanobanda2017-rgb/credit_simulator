using SimulationService.Models;

namespace SimulationService.Calculators;

public class GermanCalculator : IAmortizationCalculator
{
	public List<Installment> Calculate(decimal amount, decimal annualRate, int months)
	{
		var monthlyRate = annualRate / 12m / 100m;
		var fixedPrincipal = decimal.Round(amount / months, 2, MidpointRounding.AwayFromZero);
		var balance = amount;
		var result = new List<Installment>(months);
		for (var number = 1; number <= months; number++)
		{
			var interest = decimal.Round(balance * monthlyRate, 2, MidpointRounding.AwayFromZero);
			var principal = number == months ? balance : fixedPrincipal;
			principal = decimal.Round(principal, 2, MidpointRounding.AwayFromZero);
			var payment = decimal.Round(principal + interest, 2, MidpointRounding.AwayFromZero);
			balance = decimal.Round(balance - principal, 2, MidpointRounding.AwayFromZero);
			if (balance < 0) balance = 0;
			result.Add(new Installment { Number = number, Payment = payment, Interest = interest, Principal = principal, Balance = balance });
		}
		return result;
	}
}
