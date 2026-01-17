using Microsoft.EntityFrameworkCore;

namespace ShevaTahanotNotifier.ExtensionMethods;

public static class DbContextExtensions
{
    public static async Task<IResult> MigrateAsync(this DbContext context)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();
            await context.Database.MigrateAsync();
            return Results.Ok("Database migrated successfully.");
        }
        catch (Exception ex)
        {
            return Results.Problem("Migration failed", ex.Message);
        }
    }
}