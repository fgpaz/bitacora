using NuestrasCuentitas.Bitacora.Domain.Entities;

namespace NuestrasCuentitas.Bitacora.Tests;

public sealed class UserAuthSubjectTests
{
    [Fact]
    public void LinkAuthSubject_replaces_subject_after_zitadel_link()
    {
        var user = User.CreatePatient(
            "initial-subject",
            [1, 2, 3],
            "email-hash",
            1,
            DateTime.UtcNow);

        user.LinkAuthSubject("zitadel-subject");

        Assert.Equal("zitadel-subject", user.AuthSubject);
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
    }
}
