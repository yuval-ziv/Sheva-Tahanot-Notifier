using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Serilog;
using ShevaTahanotNotifier.Configuration;
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
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace ShevaTahanotNotifier.ExtensionMethods;

public static class WebApplicationBuilderExtensions
{
    public static void RegisterShevaTahanotNotifier(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        ConfigurationManager configuration = builder.Configuration;

        AddSerilog(services, configuration);
        AddDatabase(services);
        AddControllersWithNewtonsoftJson(services);
        AddOpenApi(services);
        AddOptions(services, configuration);
        AddNotifiers(services);
        AddServices(services);
        services.AddHttpClient();
        services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Flags = HybridCacheEntryFlags.DisableDistributedCache,
        });

        AddTelegram(services);
    }

    private static void AddDatabase(IServiceCollection services)
    {
        services.AddDbContext<NotifierContext>();
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
    }

    private static void AddControllersWithNewtonsoftJson(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; });
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
        services.AddScoped<INotifierService, TelegramNotifierService>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IBridgeStatusFetcher, HtmlBridgeStatusFetcher>();
        services.AddScoped<IBridgeStatusService, BridgeStatusService>();
        services.AddScoped<IAdminUserValidatorService, AdminUserValidatorService>();
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
        services.AddHostedService<BackgroundPollingService>();

        AddCommandHandlers(services);
        AddCallbackHandlers(services);
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
    }

    private static void AddCallbackHandlers(IServiceCollection services)
    {
        services.AddScoped<ICallbackHandler, AddCallbackHandler>();
    }
}