namespace CreditCatalogService.Models;

public class CreditType
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public decimal AnnualRate { get; set; }
	public decimal MinAmount { get; set; }
	public decimal MaxAmount { get; set; }
	public int MaxMonths { get; set; }
	public bool IsActive { get; set; }
}
