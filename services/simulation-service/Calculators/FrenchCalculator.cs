using SimulationService.Models;

namespace SimulationService.Calculators;

public class FrenchCalculator : IAmortizationCalculator
{
	public List<Installment> Calculate(decimal amount, decimal annualRate, int months)
	{
		var monthlyRate = annualRate / 12m / 100m;
		var payment = monthlyRate == 0 ? amount / months : amount * monthlyRate / (1m - (decimal)Math.Pow(1 + (double)monthlyRate, -months));
		payment = decimal.Round(payment, 2, MidpointRounding.AwayFromZero);
		var balance = amount;
		var result = new List<Installment>(months);
		for (var number = 1; number <= months; number++)
		{
			var interest = decimal.Round(balance * monthlyRate, 2, MidpointRounding.AwayFromZero);
			var principal = decimal.Round(payment - interest, 2, MidpointRounding.AwayFromZero);
			var actualPayment = payment;
			if (number == months)
			{
				principal = decimal.Round(balance, 2, MidpointRounding.AwayFromZero);
				actualPayment = decimal.Round(principal + interest, 2, MidpointRounding.AwayFromZero);
			}
			balance = decimal.Round(balance - principal, 2, MidpointRounding.AwayFromZero);
			if (balance < 0) balance = 0;
			result.Add(new Installment { Number = number, Payment = actualPayment, Interest = interest, Principal = principal, Balance = balance });
		}
		return result;
	}
}
