using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Transactions;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Context;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;

namespace NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BitacoraDb")
            ?? throw new InvalidOperationException("ConnectionStrings:BitacoraDb is required.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<ICurrentPatientContextAccessor, NullCurrentPatientContextAccessor>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IConsentGrantRepository, ConsentGrantRepository>();
        services.AddScoped<IMoodEntryRepository, MoodEntryRepository>();
        services.AddScoped<IDailyCheckinRepository, DailyCheckinRepository>();
        services.AddScoped<IPendingInviteRepository, PendingInviteRepository>();
        services.AddScoped<IAccessAuditRepository, AccessAuditRepository>();
        services.AddScoped<IBindingCodeRepository, BindingCodeRepository>();
        services.AddScoped<ICareLinkRepository, CareLinkRepository>();
        services.AddScoped<ITelegramPairingCodeRepository, TelegramPairingCodeRepository>();
        services.AddScoped<ITelegramSessionRepository, TelegramSessionRepository>();
        services.AddScoped<IReminderConfigRepository, ReminderConfigRepository>();
        services.AddScoped<IAnalyticsEventRepository, AnalyticsEventRepository>();
        services.AddScoped<IBitacoraUnitOfWork, EntityFrameworkBitacoraUnitOfWork>();

        return services;
    }
}

