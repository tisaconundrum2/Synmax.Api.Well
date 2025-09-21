using Microsoft.EntityFrameworkCore;
using Synmax.Api.Well.Data;
using Synmax.Api.Well.Models;

namespace Synmax.Api.Well.Services
{
    public class DatabaseSeeder
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(IServiceScopeFactory scopeFactory, ILogger<DatabaseSeeder> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (await HasBeenSeededAsync(dbContext))
            {
                _logger.LogInformation("Database has already been seeded. Skipping seed operation.");
                return;
            }

            try
            {
                await SeedDataAsync(dbContext);
                await MarkAsSeededAsync(dbContext);
                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task<bool> HasBeenSeededAsync(ApplicationDbContext dbContext)
        {
            return await dbContext.DatabaseSeeded.AnyAsync();
        }

        private async Task MarkAsSeededAsync(ApplicationDbContext dbContext)
        {
            dbContext.DatabaseSeeded.Add(new DatabaseSeeded
            {
                SeededAt = DateTime.UtcNow,
                Description = "Initial data seed"
            });
            await dbContext.SaveChangesAsync();
        }

        private async Task SeedDataAsync(ApplicationDbContext dbContext)
        {
            // TODO: Add your data seeding logic here
            // Example:
            // if (!await dbContext.YourTable.AnyAsync())
            // {
            //     dbContext.YourTable.AddRange(new[]
            //     {
            //         new YourEntity { ... },
            //         new YourEntity { ... }
            //     });
            //     await dbContext.SaveChangesAsync();
            // }
        }
    }
}