using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging.Abstractions;
using NuestrasCuentitas.Bitacora.Application.Commands.Telegram;
using NuestrasCuentitas.Bitacora.Application.Common;
using NuestrasCuentitas.Bitacora.Application.Queries.Telegram;
using NuestrasCuentitas.Bitacora.DataAccess.EntityFramework.Persistence;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Repositories;
using NuestrasCuentitas.Bitacora.DataAccess.Interface.Transactions;
using NuestrasCuentitas.Bitacora.Domain.Entities;
using NuestrasCuentitas.Bitacora.Domain.Enums;

namespace NuestrasCuentitas.Bitacora.Tests;

public sealed class ReminderScheduleTests
{
    private static readonly Guid PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private const string BuenosAiresTimezone = "America/Argentina/Buenos_Aires";

    [Fact]
    public async Task ConfigureReminderSchedule_requires_active_telegram_session()
    {
        var handler = CreateHandler(
            new FakeReminderConfigRepository(),
            new FakeTelegramSessionRepository());

        var ex = await Assert.ThrowsAsync<BitacoraException>(() =>
            handler.Handle(CreateCommand(22, 0), CancellationToken.None).AsTask());

        Assert.Equal("TG_006_NO_ACTIVE_SESSION", ex.Code);
        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public async Task ConfigureReminderSchedule_rejects_invalid_hour()
    {
        var reminderRepository = new FakeReminderConfigRepository();
        var handler = CreateHandler(
            reminderRepository,
            new FakeTelegramSessionRepository(TelegramSession.CreateLinked(PatientId, "test-chat", DateTime.UtcNow)));

        var ex = await Assert.ThrowsAsync<BitacoraException>(() =>
            handler.Handle(CreateCommand(24, 0), CancellationToken.None).AsTask());

        Assert.Equal("TG_006_INVALID_HOUR", ex.Code);
        Assert.Equal(400, ex.StatusCode);
        Assert.Equal(0, reminderRepository.AddedCount);
        Assert.Equal(0, reminderRepository.UpdatedCount);
    }

    [Fact]
    public async Task ConfigureReminderSchedule_rejects_invalid_minute()
    {
        var reminderRepository = new FakeReminderConfigRepository();
        var handler = CreateHandler(
            reminderRepository,
            new FakeTelegramSessionRepository(TelegramSession.CreateLinked(PatientId, "test-chat", DateTime.UtcNow)));

        var ex = await Assert.ThrowsAsync<BitacoraException>(() =>
            handler.Handle(CreateCommand(22, 15), CancellationToken.None).AsTask());

        Assert.Equal("TG_006_INVALID_MINUTE", ex.Code);
        Assert.Equal(400, ex.StatusCode);
        Assert.Equal(0, reminderRepository.AddedCount);
        Assert.Equal(0, reminderRepository.UpdatedCount);
    }

    [Fact]
    public async Task ConfigureReminderSchedule_rejects_invalid_timezone()
    {
        var reminderRepository = new FakeReminderConfigRepository();
        var handler = CreateHandler(
            reminderRepository,
            new FakeTelegramSessionRepository(TelegramSession.CreateLinked(PatientId, "test-chat", DateTime.UtcNow)));

        var ex = await Assert.ThrowsAsync<BitacoraException>(() =>
            handler.Handle(CreateCommand(22, 0, "Mars/Olympus"), CancellationToken.None).AsTask());

        Assert.Equal("TG_006_INVALID_TIMEZONE", ex.Code);
        Assert.Equal(400, ex.StatusCode);
        Assert.Equal(0, reminderRepository.AddedCount);
        Assert.Equal(0, reminderRepository.UpdatedCount);
    }

    [Fact]
    public async Task ConfigureReminderSchedule_defaults_timezone_to_buenos_aires()
    {
        var reminderRepository = new FakeReminderConfigRepository();
        var unitOfWork = new FakeBitacoraUnitOfWork();
        var handler = CreateHandler(
            reminderRepository,
            new FakeTelegramSessionRepository(TelegramSession.CreateLinked(PatientId, "test-chat", DateTime.UtcNow)),
            unitOfWork);

        var response = await handler.Handle(
            CreateCommand(1, 0, timezone: null),
            CancellationToken.None);

        Assert.Equal(1, response.HourUtc);
        Assert.Equal(0, response.MinuteUtc);
        Assert.Equal(BuenosAiresTimezone, response.ReminderTimezone);
        Assert.True(response.Enabled);
        Assert.NotNull(response.NextFireAtUtc);
        Assert.Equal(1, reminderRepository.AddedCount);
        Assert.Equal(1, unitOfWork.SaveChangesCount);
    }

    [Fact]
    public void ReminderConfig_reschedule_reenables_disabled_config()
    {
        var now = new DateTime(2026, 4, 20, 12, 0, 0, DateTimeKind.Utc);
        var config = ReminderConfig.CreateDefault(PatientId, 9, 0, BuenosAiresTimezone);
        config.Disable(now);

        config.ConfigureSchedule(1, 0, BuenosAiresTimezone, now);

        Assert.True(config.Enabled);
        Assert.Null(config.DisabledAtUtc);
        Assert.Equal(1, config.HourUtc);
        Assert.Equal(0, config.MinuteUtc);
        Assert.Equal(BuenosAiresTimezone, config.ReminderTimezone);
        Assert.NotNull(config.NextFireAtUtc);
    }

    [Fact]
    public async Task GetReminderSchedule_returns_not_configured_when_missing()
    {
        var handler = new GetReminderScheduleQueryHandler(new FakeReminderConfigRepository());

        var response = await handler.Handle(
            new GetReminderScheduleQuery(PatientId, Guid.Parse("22222222-2222-2222-2222-222222222222")),
            CancellationToken.None);

        Assert.False(response.Configured);
        Assert.Null(response.ReminderConfigId);
        Assert.Null(response.HourUtc);
        Assert.Null(response.MinuteUtc);
        Assert.Null(response.ReminderTimezone);
        Assert.Null(response.Enabled);
        Assert.Null(response.NextFireAtUtc);
    }

    [Fact]
    public async Task GetReminderSchedule_returns_existing_config_for_patient()
    {
        var config = ReminderConfig.CreateDefault(PatientId, 1, 0, BuenosAiresTimezone);
        var handler = new GetReminderScheduleQueryHandler(new FakeReminderConfigRepository(config));

        var response = await handler.Handle(
            new GetReminderScheduleQuery(PatientId, Guid.Parse("22222222-2222-2222-2222-222222222222")),
            CancellationToken.None);

        Assert.True(response.Configured);
        Assert.Equal(config.ReminderConfigId, response.ReminderConfigId);
        Assert.Equal(1, response.HourUtc);
        Assert.Equal(0, response.MinuteUtc);
        Assert.Equal(BuenosAiresTimezone, response.ReminderTimezone);
        Assert.True(response.Enabled);
        Assert.Equal(config.NextFireAtUtc, response.NextFireAtUtc);
    }

    [Fact]
    public void AppDbContext_maps_reminder_timezone_to_snake_case_column()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=bitacora_test;Username=test")
            .Options;

        using var dbContext = new AppDbContext(options);
        var entity = dbContext.Model.FindEntityType(typeof(ReminderConfig));
        Assert.NotNull(entity);

        var table = StoreObjectIdentifier.Table("reminder_configs", schema: null);
        var property = entity!.FindProperty(nameof(ReminderConfig.ReminderTimezone));

        Assert.NotNull(property);
        Assert.Equal("reminder_timezone", property!.GetColumnName(table));
    }

    [Fact]
    public void AppDbContext_limits_telegram_chat_unique_index_to_linked_sessions()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=bitacora_test;Username=test")
            .Options;

        using var dbContext = new AppDbContext(options);
        var entity = dbContext.Model.FindEntityType(typeof(TelegramSession));
        Assert.NotNull(entity);

        var index = entity!.GetIndexes().SingleOrDefault(candidate =>
            candidate.Properties.Any(property => property.Name == nameof(TelegramSession.ChatId)));

        Assert.NotNull(index);
        Assert.True(index!.IsUnique);
        Assert.Equal("status = 'Linked'", index.GetFilter());
    }

    private static ConfigureReminderScheduleCommand CreateCommand(
        int hourUtc,
        int minuteUtc,
        string? timezone = BuenosAiresTimezone)
    {
        return new ConfigureReminderScheduleCommand(
            PatientId,
            hourUtc,
            minuteUtc,
            timezone,
            Guid.Parse("22222222-2222-2222-2222-222222222222"));
    }

    private static ConfigureReminderScheduleCommandHandler CreateHandler(
        FakeReminderConfigRepository reminderRepository,
        FakeTelegramSessionRepository telegramSessionRepository,
        FakeBitacoraUnitOfWork? unitOfWork = null)
    {
        return new ConfigureReminderScheduleCommandHandler(
            reminderRepository,
            telegramSessionRepository,
            unitOfWork ?? new FakeBitacoraUnitOfWork(),
            NullLogger<ConfigureReminderScheduleCommandHandler>.Instance);
    }

    private sealed class FakeReminderConfigRepository(ReminderConfig? existing = null) : IReminderConfigRepository
    {
        private ReminderConfig? _config = existing;

        public int AddedCount { get; private set; }
        public int UpdatedCount { get; private set; }

        public Task<ReminderConfig?> GetByIdAsync(Guid reminderConfigId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_config?.ReminderConfigId == reminderConfigId ? _config : null);
        }

        public Task<ReminderConfig?> FindByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_config?.PatientId == patientId ? _config : null);
        }

        public Task AddAsync(ReminderConfig config, CancellationToken cancellationToken = default)
        {
            _config = config;
            AddedCount++;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(ReminderConfig config, CancellationToken cancellationToken = default)
        {
            _config = config;
            UpdatedCount++;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<ReminderConfig>> GetDueRemindersAsync(
            DateTime asOfUtc,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ReminderConfig> configs = _config is { Enabled: true, NextFireAtUtc: not null }
                && _config.NextFireAtUtc <= asOfUtc
                    ? [_config]
                    : [];

            return Task.FromResult(configs);
        }
    }

    private sealed class FakeTelegramSessionRepository(TelegramSession? linkedSession = null) : ITelegramSessionRepository
    {
        public Task<TelegramSession?> GetByIdAsync(Guid telegramSessionId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(linkedSession?.TelegramSessionId == telegramSessionId ? linkedSession : null);
        }

        public Task<TelegramSession?> FindLinkedByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(linkedSession?.PatientId == patientId
                && linkedSession.Status == TelegramSessionStatus.Linked
                    ? linkedSession
                    : null);
        }

        public Task<TelegramSession?> FindLinkedByChatIdAsync(string chatId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(linkedSession?.ChatId == chatId
                && linkedSession.Status == TelegramSessionStatus.Linked
                    ? linkedSession
                    : null);
        }

        public Task<TelegramSession?> GetByChatIdAsync(string chatId, CancellationToken cancellationToken = default)
        {
            return FindLinkedByChatIdAsync(chatId, cancellationToken);
        }

        public Task AddAsync(TelegramSession session, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TelegramSession session, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeBitacoraUnitOfWork : IBitacoraUnitOfWork
    {
        public int SaveChangesCount { get; private set; }

        public Task<IBitacoraTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IBitacoraTransaction>(new FakeBitacoraTransaction());
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeBitacoraTransaction : IBitacoraTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
