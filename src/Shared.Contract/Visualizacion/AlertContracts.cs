namespace Shared.Contract.Visualizacion;

public sealed record PatientAlertDto(
    string Code,
    string Severity,
    string Message,
    DateOnly TriggerDate,
    int ConsecutiveDays);
