namespace Synmax.Api.Well.Services
{
    public class DatabaseSeederHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DatabaseSeederHostedService> _logger;

        public DatabaseSeederHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<DatabaseSeederHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");
                using var scope = _scopeFactory.CreateScope();
                var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}