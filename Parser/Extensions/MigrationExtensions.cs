using Microsoft.EntityFrameworkCore;
using Parser.Data;

namespace Products.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        
        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Проверяем, есть ли неподтверждённые миграции
        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        
        if (pendingMigrations.Any())
        {
            Console.WriteLine("Применяются новые миграции...");
            dbContext.Database.Migrate();
        }
        else
        {
            Console.WriteLine("Миграции не требуются.");
        }
    }
}