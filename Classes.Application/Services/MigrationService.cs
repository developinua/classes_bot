using System;
using System.Threading.Tasks;
using Classes.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Classes.Application.Services;

public static class MigrationService
{
    public static async Task MigrateDbAsync(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<BotService>>();
        
        try
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Successfully migrated the database.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }
}