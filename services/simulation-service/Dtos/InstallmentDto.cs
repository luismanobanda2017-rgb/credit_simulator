namespace SimulationService.Dtos;

public record InstallmentDto(int Number, decimal Payment, decimal Interest, decimal Principal, decimal Balance);
