using Microsoft.EntityFrameworkCore;
using Serilog;
using ShevaTahanotNotifier.Database;

namespace ShevaTahanotNotifier.ExtensionMethods;

public static class WebApplicationExtensions
{
    public static async Task MapShevaTahanotNotifier(this WebApplication app, CancellationToken cancellationToken = default)
    {
        app.MapOpenApi();
        app.MapControllers();
        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();

        await RunMigrationsAsync(app.Services, cancellationToken);
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
}