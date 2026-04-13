namespace NuestrasCuentitas.Bitacora.Domain.Enums;

/// <summary>
/// Tracks the Telegram conversation step for the sequential mood + factors flow (RF-REG-013).
/// </summary>
public enum TelegramConversationState
{
    /// <summary>Default idle state — expecting a mood score or /start.</summary>
    Idle = 0,

    /// <summary>Patient sent a mood score; awaiting factor responses (sleep_hours).</summary>
    AwaitingFactorSleep = 1,

    /// <summary>Awaiting physical activity answer (si/no).</summary>
    AwaitingFactorPhysical = 2,

    /// <summary>Awaiting social activity answer (si/no).</summary>
    AwaitingFactorSocial = 3,

    /// <summary>Awaiting anxiety answer (si/no).</summary>
    AwaitingFactorAnxiety = 4,

    /// <summary>Awaiting irritability answer (si/no).</summary>
    AwaitingFactorIrritability = 5,

    /// <summary>Awaiting medication answer (si/no).</summary>
    AwaitingFactorMedication = 6,

    /// <summary>Awaiting medication time input (HH:mm) after medication answer was "si".</summary>
    AwaitingFactorMedicationTime = 7
}
