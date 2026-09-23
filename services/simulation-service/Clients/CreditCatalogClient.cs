using System.Net.Http.Json;

namespace SimulationService.Clients;

public record CatalogCreditType(int Id, string Name, decimal AnnualRate, decimal MinAmount, decimal MaxAmount, int MaxMonths);

public class CreditCatalogClient(HttpClient httpClient)
{
	public async Task<CatalogCreditType?> GetByIdAsync(int id, CancellationToken cancellationToken)
	{
		return await httpClient.GetFromJsonAsync<CatalogCreditType>($"api/credittypes/{id}", cancellationToken);
	}
}
