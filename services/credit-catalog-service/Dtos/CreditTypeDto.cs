namespace CreditCatalogService.Dtos;

public record CreditTypeDto(int Id, string Name, decimal AnnualRate, decimal MinAmount, decimal MaxAmount, int MaxMonths);
