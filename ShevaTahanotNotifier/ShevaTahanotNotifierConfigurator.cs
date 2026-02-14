using Coravel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Serilog;
using ShevaTahanotNotifier.BackgroundServices;
using ShevaTahanotNotifier.Configuration;
using ShevaTahanotNotifier.Coravel;
using ShevaTahanotNotifier.Database;
using ShevaTahanotNotifier.Database.Entities;
using ShevaTahanotNotifier.Database.Repositories;
using ShevaTahanotNotifier.Database.Repositories.Abstract;
using ShevaTahanotNotifier.Services;
using ShevaTahanotNotifier.Services.Notifiers;
using ShevaTahanotNotifier.Telegram;
using ShevaTahanotNotifier.Telegram.CallbackHandlers;
using ShevaTahanotNotifier.Telegram.CommandHandlers;
using ShevaTahanotNotifier.Telegram.CommandHandlers.Abstraction;
using ShevaTahanotNotifier.Telegram.ConversationHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace ShevaTahanotNotifier;

public class ShevaTahanotNotifierConfigurator
{
    public static async Task<WebApplication> ConfigureAsync(WebApplicationBuilder builder, CancellationToken cancellationToken = default)
    {
        ConfigureServices(builder);

        return await BuildApplication(builder, cancellationToken);
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        ConfigurationManager configuration = builder.Configuration;

        AddSerilog(services, configuration);
        AddDatabase(services);
        AddOpenApi(services);
        AddOptions(services, configuration);
        AddNotifiers(services);
        AddServices(services);
        AddBackgroundServices(services);
        AddCoravel(services);
        services.AddHttpClient();
        services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Flags = HybridCacheEntryFlags.DisableDistributedCache,
        });

        AddTelegram(services);
    }

    private static void AddDatabase(IServiceCollection services)
    {
        services.AddDbContext<NotifierContext>(options => options.UseLazyLoadingProxies());
        AddRepositories(services);
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IGenericRepository<User>, UserRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();

        services.AddScoped<IGenericRepository<NotificationSchedule>, NotificationScheduleRepository>();
        services.AddScoped<INotificationScheduleRepository, NotificationScheduleRepository>();

        services.AddScoped<IGenericRepository<BridgeStatus>, BridgeStatusRepository>();
        services.AddScoped<IBridgeStatusRepository, BridgeStatusRepository>();

        services.AddScoped<IGenericRepository<Conversation>, ConversationRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
    }

    private static void AddOpenApi(IServiceCollection services)
    {
        services.AddOpenApi(options => { options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1; });
    }

    private static void AddOptions(IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<NotifierContextOptions>(configuration.GetSection(NotifierContextOptions.ConfigurationSectionName));
        services.Configure<TelegramBotOptions>(configuration.GetSection(TelegramBotOptions.ConfigurationSectionName));
        services.Configure<HtmlBridgeStatusFetcherOptions>(configuration.GetSection(HtmlBridgeStatusFetcherOptions.ConfigurationSectionName));
    }

    private static void AddNotifiers(IServiceCollection services)
    {
        services.AddScoped<INotifierManager, NotifierManager>();

        services.AddScoped<INotificationProviderService, TelegramNotificationProviderService>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IBridgeStatusFetcher, HtmlBridgeStatusFetcher>();
        services.AddScoped<IBridgeStatusService, BridgeStatusService>();
        services.AddScoped<IAdminUserValidatorService, AdminUserValidatorService>();
        services.AddScoped<INotificationScheduleService, NotificationScheduleService>();
    }

    private static void AddBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<BackgroundBridgeStatusPollingService>();
    }

    private static void AddCoravel(IServiceCollection services)
    {
        services.AddScoped<ICoravelService, CoravelService>();
        services.AddScheduler();
        // services.AddScoped<NotifierInvocable>();
    }

    private static void AddSerilog(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSerilog(loggerConfiguration => loggerConfiguration.ReadFrom.Configuration(configuration));
    }

    private static void AddTelegram(IServiceCollection services)
    {
        services.AddHttpClient("telegram_bot_client").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
        {
            TelegramBotOptions botConfiguration = serviceProvider.GetRequiredService<IOptions<TelegramBotOptions>>().Value;
            TelegramBotClientOptions options = new(botConfiguration.BotToken);
            return new TelegramBotClient(options, httpClient);
        });

        services.AddScoped<IUpdateHandler, ShevaTahanotNotifierUpdateHandler>();
        services.AddScoped<ITelegramReceiverService, TelegramReceiverService>();
        services.AddScoped<IBotCommandHelper, BotCommandHelper>();
        services.AddHostedService<BackgroundTelegramPollingService>();

        services.AddScoped<IFreeTextMessageHandler, FreeTextMessageHandler>();
        AddCommandHandlers(services);
        AddCallbackHandlers(services);
        AddConversationHandlers(services);
    }

    private static void AddCommandHandlers(IServiceCollection services)
    {
        services.AddScoped<HelpCommandHandler>();
        services.AddScoped<ICommandHandler, RegisterCommandHandler>();
        services.AddScoped<ICommandHandler, DeleteCommandHandler>();
        services.AddScoped<ICommandHandler, AddNotificationScheduleCommandHandler>();
        services.AddScoped<ICommandHandler, RemoveNotificationScheduleCommandHandler>();
        services.AddScoped<ICommandHandler, EnableNotificationScheduleCommandHandler>();
        services.AddScoped<ICommandHandler, DisableNotificationScheduleCommandHandler>();
        services.AddScoped<ICommandHandler, StatusCommandHandler>();
        services.AddScoped<ICommandHandler, RefreshCommandHandler>();
        services.AddScoped<ICommandHandler, ListNotificationScheduleCommandHandler>();
    }

    private static void AddCallbackHandlers(IServiceCollection services)
    {
        services.AddScoped<ICallbackHandler, AddNotificationDayCallbackHandler>();
        services.AddScoped<ICallbackHandler, RemoveNotificationCallbackHandler>();
        services.AddScoped<ICallbackHandler, EnableNotificationCallbackHandler>();
        services.AddScoped<ICallbackHandler, DisableNotificationCallbackHandler>();
    }

    private static void AddConversationHandlers(IServiceCollection services)
    {
        services.AddScoped<IConversationHandler, AddHourConversationHandler>();
    }


    private static async Task<WebApplication> BuildApplication(WebApplicationBuilder builder, CancellationToken cancellationToken)
    {
        WebApplication app = builder.Build();

        app.MapOpenApi();
        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();

        await RunMigrationsAsync(app.Services, cancellationToken);
        await InitializeCoravelAsync(app.Services, cancellationToken);
        return app;
    }

    private static async Task RunMigrationsAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        try
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<NotifierContext>();
            await context.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database");
        }
    }

    private static async Task InitializeCoravelAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        var coravelService = services.CreateAsyncScope().ServiceProvider.GetRequiredService<ICoravelService>();
        await coravelService.InitializeAsync(cancellationToken);
    }
}