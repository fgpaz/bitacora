namespace NuestrasCuentitas.Bitacora.Application.Interfaces;

public interface IPseudonymizationService
{
    string CreatePseudonym(Guid actorId);
}
