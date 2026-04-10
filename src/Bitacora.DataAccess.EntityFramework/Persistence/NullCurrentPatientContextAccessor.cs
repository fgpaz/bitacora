using NuestrasCuentitas.Bitacora.DataAccess.Interface.Context;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;

public sealed class NullCurrentPatientContextAccessor : ICurrentPatientContextAccessor
{
    public Guid? PatientId => null;
}
