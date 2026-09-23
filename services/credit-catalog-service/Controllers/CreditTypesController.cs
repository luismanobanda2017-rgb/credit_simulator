using CreditCatalogService.Data;
using CreditCatalogService.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditCatalogService.Controllers;

[ApiController]
[Route("api/credittypes")]
public class CreditTypesController(CatalogDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<IEnumerable<CreditTypeDto>>> GetActive(CancellationToken cancellationToken)
	{
		var types = await db.CreditTypes.AsNoTracking()
			.Where(type => type.IsActive)
			.OrderBy(type => type.Name)
			.Select(type => new CreditTypeDto(type.Id, type.Name, type.AnnualRate, type.MinAmount, type.MaxAmount, type.MaxMonths))
			.ToListAsync(cancellationToken);
		return Ok(types);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<CreditTypeDto>> GetById(int id, CancellationToken cancellationToken)
	{
		var type = await db.CreditTypes.AsNoTracking().Where(item => item.Id == id && item.IsActive)
			.Select(item => new CreditTypeDto(item.Id, item.Name, item.AnnualRate, item.MinAmount, item.MaxAmount, item.MaxMonths))
			.SingleOrDefaultAsync(cancellationToken);
		return type is null ? NotFound(new { message = "Tipo de crédito no encontrado." }) : Ok(type);
	}
}
