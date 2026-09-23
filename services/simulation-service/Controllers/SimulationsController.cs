using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimulationService.Calculators;
using SimulationService.Clients;
using SimulationService.Data;
using SimulationService.Dtos;
using SimulationService.Models;
using SimulationService.Services;

namespace SimulationService.Controllers;

[ApiController]
[Authorize]
[Route("api/simulations")]
public class SimulationsController(SimulationDbContext db, CreditCatalogClient catalogClient, ReportExporter reportExporter, FrenchCalculator french, GermanCalculator german) : ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<SimulationDto>> Create(SimulationRequest request, CancellationToken cancellationToken)
	{
		var method = request.Method.Trim();
		if (!method.Equals("French", StringComparison.OrdinalIgnoreCase) && !method.Equals("German", StringComparison.OrdinalIgnoreCase))
			return BadRequest(new { message = "El método debe ser French o German." });
		CatalogCreditType? type;
		try { type = await catalogClient.GetByIdAsync(request.CreditTypeId, cancellationToken); }
		catch (HttpRequestException) { return StatusCode(503, new { message = "El catálogo de créditos no está disponible." }); }
		if (type is null) return BadRequest(new { message = "Tipo de crédito inválido." });
		if (request.Amount < type.MinAmount || request.Amount > type.MaxAmount) return BadRequest(new { message = $"El monto debe estar entre {type.MinAmount:F2} y {type.MaxAmount:F2}." });
		if (request.Months > type.MaxMonths) return BadRequest(new { message = $"El plazo máximo es de {type.MaxMonths} meses." });
		var userId = GetUserId();
		var calculator = method.Equals("French", StringComparison.OrdinalIgnoreCase) ? (IAmortizationCalculator)french : german;
		var installments = calculator.Calculate(request.Amount, type.AnnualRate, request.Months);
		var simulation = new Simulation
		{
			UserId = userId, CreditTypeId = type.Id, CreditTypeName = type.Name, Amount = request.Amount, Months = request.Months,
			AnnualRate = type.AnnualRate, Method = method.Equals("French", StringComparison.OrdinalIgnoreCase) ? "French" : "German",
			Installments = installments, TotalInterest = installments.Sum(item => item.Interest), TotalPaid = installments.Sum(item => item.Payment)
		};
		db.Simulations.Add(simulation);
		await db.SaveChangesAsync(cancellationToken);
		return Ok(ToDto(simulation));
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<SimulationDto>>> GetHistory(CancellationToken cancellationToken)
	{
		var userId = GetUserId();
		var items = await db.Simulations.AsNoTracking().Where(item => item.UserId == userId).OrderByDescending(item => item.CreatedAt).ToListAsync(cancellationToken);
		return Ok(items.Select(ToDto));
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<SimulationDto>> GetById(int id, CancellationToken cancellationToken)
	{
		var item = await db.Simulations.AsNoTracking().Include(simulation => simulation.Installments).SingleOrDefaultAsync(simulation => simulation.Id == id && simulation.UserId == GetUserId(), cancellationToken);
		return item is null ? NotFound(new { message = "Simulación no encontrada." }) : Ok(ToDto(item));
	}

	[HttpGet("{id:int}/export")]
	public async Task<IActionResult> Export(int id, [FromQuery] string format = "pdf", CancellationToken cancellationToken = default)
	{
		var item = await db.Simulations.AsNoTracking().Include(simulation => simulation.Installments).SingleOrDefaultAsync(simulation => simulation.Id == id && simulation.UserId == GetUserId(), cancellationToken);
		if (item is null) return NotFound(new { message = "Simulación no encontrada." });
		try { var report = reportExporter.Export(item, format); return File(report.Content, report.ContentType, report.FileName); }
		catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
	}

	private Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : throw new UnauthorizedAccessException("El token no contiene un usuario válido.");

	private static SimulationDto ToDto(Simulation item) => new()
	{
		Id = item.Id, UserId = item.UserId, CreditTypeId = item.CreditTypeId, CreditTypeName = item.CreditTypeName, Amount = item.Amount, Months = item.Months,
		AnnualRate = item.AnnualRate, Method = item.Method, TotalInterest = item.TotalInterest, TotalPaid = item.TotalPaid, CreatedAt = item.CreatedAt,
		Installments = item.Installments.OrderBy(installment => installment.Number).Select(installment => new InstallmentDto(installment.Number, installment.Payment, installment.Interest, installment.Principal, installment.Balance)).ToList()
	};
}
