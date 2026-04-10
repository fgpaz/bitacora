namespace NuestrasCuentitas.Bitacora.DataAccess.Interface.Context;

public interface ICurrentPatientContextAccessor
{
    Guid? PatientId { get; }
}
