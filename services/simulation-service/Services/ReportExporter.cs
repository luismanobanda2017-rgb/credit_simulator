using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SimulationService.Models;

namespace SimulationService.Services;

public record ExportedReport(byte[] Content, string ContentType, string FileName);

public class ReportExporter
{
	public ExportedReport Export(Simulation simulation, string format)
	{
		return format.ToLowerInvariant() switch
		{
			"csv" => Csv(simulation),
			"xlsx" or "excel" => Excel(simulation),
			"pdf" => Pdf(simulation),
			_ => throw new ArgumentException("Formato no soportado. Usa pdf, xlsx o csv.")
		};
	}

	private static ExportedReport Csv(Simulation simulation)
	{
		var builder = new StringBuilder();
		builder.AppendLine("Tipo de crédito,Monto,Plazo,Tasa anual,Método,Fecha");
		builder.AppendLine(string.Join(',', CsvValue(simulation.CreditTypeName), simulation.Amount.ToString("F2", CultureInfo.InvariantCulture), simulation.Months, simulation.AnnualRate.ToString("F2", CultureInfo.InvariantCulture), CsvValue(simulation.Method), simulation.CreatedAt.ToString("u")));
		builder.AppendLine();
		builder.AppendLine("Número,Cuota,Interés,Capital,Saldo");
		foreach (var item in simulation.Installments.OrderBy(item => item.Number))
			builder.AppendLine($"{item.Number},{item.Payment:F2},{item.Interest:F2},{item.Principal:F2},{item.Balance:F2}");
		builder.AppendLine($"Totales,{simulation.TotalPaid:F2},{simulation.TotalInterest:F2},{simulation.Amount:F2},0.00");
		return new ExportedReport(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(builder.ToString())).ToArray(), "text/csv", $"simulacion-{simulation.Id}.csv");
	}

	private static ExportedReport Excel(Simulation simulation)
	{
		using var stream = new MemoryStream();
		using (var workbook = new XLWorkbook())
		{
			var sheet = workbook.Worksheets.Add("Amortización");
			sheet.Cell(1, 1).Value = "Simulación de crédito";
			sheet.Cell(2, 1).Value = "Tipo"; sheet.Cell(2, 2).Value = simulation.CreditTypeName;
			sheet.Cell(3, 1).Value = "Monto"; sheet.Cell(3, 2).Value = simulation.Amount;
			sheet.Cell(4, 1).Value = "Plazo"; sheet.Cell(4, 2).Value = simulation.Months;
			sheet.Cell(5, 1).Value = "Tasa anual"; sheet.Cell(5, 2).Value = simulation.AnnualRate;
			sheet.Cell(6, 1).Value = "Método"; sheet.Cell(6, 2).Value = simulation.Method;
			var row = 8;
			sheet.Cell(row, 1).Value = "Número"; sheet.Cell(row, 2).Value = "Cuota"; sheet.Cell(row, 3).Value = "Interés"; sheet.Cell(row, 4).Value = "Capital"; sheet.Cell(row, 5).Value = "Saldo";
			foreach (var item in simulation.Installments.OrderBy(item => item.Number))
			{
				row++; sheet.Cell(row, 1).Value = item.Number; sheet.Cell(row, 2).Value = item.Payment; sheet.Cell(row, 3).Value = item.Interest; sheet.Cell(row, 4).Value = item.Principal; sheet.Cell(row, 5).Value = item.Balance;
			}
			row++; sheet.Cell(row, 1).Value = "Totales"; sheet.Cell(row, 2).Value = simulation.TotalPaid; sheet.Cell(row, 3).Value = simulation.TotalInterest; sheet.Cell(row, 4).Value = simulation.Amount;
			sheet.Columns().AdjustToContents();
			workbook.SaveAs(stream);
		}
		return new ExportedReport(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"simulacion-{simulation.Id}.xlsx");
	}

	private static ExportedReport Pdf(Simulation simulation)
	{
		QuestPDF.Settings.License = LicenseType.Community;
		var bytes = Document.Create(document => document.Page(page =>
		{
			page.Margin(30);
			page.Header().Text($"Simulación de crédito #{simulation.Id}").FontSize(18).Bold();
			page.Content().Column(column =>
			{
				column.Item().Text($"Tipo: {simulation.CreditTypeName} | Monto: {simulation.Amount:F2} | Plazo: {simulation.Months} meses | Tasa: {simulation.AnnualRate:F2}% | Método: {simulation.Method}");
				column.Item().Table(table =>
				{
					table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); columns.RelativeColumn(); columns.RelativeColumn(); columns.RelativeColumn(); });
					table.Header(header => { foreach (var text in new[] { "Número", "Cuota", "Interés", "Capital", "Saldo" }) header.Cell().Text(text).Bold(); });
					foreach (var item in simulation.Installments.OrderBy(item => item.Number))
					{
						table.Cell().Text(item.Number.ToString()); table.Cell().Text(item.Payment.ToString("F2")); table.Cell().Text(item.Interest.ToString("F2")); table.Cell().Text(item.Principal.ToString("F2")); table.Cell().Text(item.Balance.ToString("F2"));
					}
					table.Cell().Text("Totales").Bold(); table.Cell().Text(simulation.TotalPaid.ToString("F2")).Bold(); table.Cell().Text(simulation.TotalInterest.ToString("F2")).Bold(); table.Cell().Text(simulation.Amount.ToString("F2")).Bold(); table.Cell().Text("0.00").Bold();
				});
			});
		})).GeneratePdf();
		return new ExportedReport(bytes, "application/pdf", $"simulacion-{simulation.Id}.pdf");
	}

	private static string CsvValue(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
}
