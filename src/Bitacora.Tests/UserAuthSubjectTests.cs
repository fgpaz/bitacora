using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.Tests;

public sealed class UserAuthSubjectTests
{
    [Fact]
    public void LinkAuthSubject_preserves_previous_subject_for_rollback()
    {
        var user = User.CreatePatient(
            "supabase-subject",
            [1, 2, 3],
            "email-hash",
            1,
            DateTime.UtcNow);

        user.LinkAuthSubject("zitadel-subject");

        Assert.Equal("zitadel-subject", user.AuthSubject);
        Assert.Equal("supabase-subject", user.LegacyAuthSubject);
    }

    [Fact]
    public void LinkAuthSubject_is_idempotent_for_same_subject()
    {
        var user = User.CreatePatient(
            "zitadel-subject",
            [1, 2, 3],
            "email-hash",
            1,
            DateTime.UtcNow);

        user.LinkAuthSubject("zitadel-subject");

        Assert.Equal("zitadel-subject", user.AuthSubject);
        Assert.Null(user.LegacyAuthSubject);
    }
}
