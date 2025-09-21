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
            if (!await dbContext.WellDetails.AnyAsync())
            {
                // use the parser to parse the website.
                // use the apis_pythondev_test.csv to get a list of api numbers to parse.
                // read the csv file
                var apiNumbers = File.ReadAllLines("apis_pythondev_test.csv")
                    .Skip(1) // skip header
                    .Select(line => line.Split(',')[0]) // get the first column (api number)
                    .ToList();

                WellDetailsParser parser = new WellDetailsParser();

                foreach (var apiNumber in apiNumbers)
                {
                    var wellDetails = await parser.ParseWellDetails(apiNumber);
                    dbContext.WellDetails.AddRange(new WellDetail
                    {
                        Operator = wellDetails.GetValueOrDefault("Operator") ?? string.Empty,
                        Status = wellDetails.GetValueOrDefault("Status") ?? string.Empty,
                        WellType = wellDetails.GetValueOrDefault("WellType") ?? string.Empty,
                        WorkType = wellDetails.GetValueOrDefault("WorkType") ?? string.Empty,
                        DirectionalStatus = wellDetails.GetValueOrDefault("DirectionalStatus") ?? string.Empty,
                        MultiLateral = wellDetails.GetValueOrDefault("MultiLateral") ?? string.Empty,
                        MineralOwner = wellDetails.GetValueOrDefault("MineralOwner") ?? string.Empty,
                        SurfaceOwner = wellDetails.GetValueOrDefault("SurfaceOwner") ?? string.Empty,
                        SurfaceLocation = string.Join(", ", new[] {
                            wellDetails.GetValueOrDefault("Location"),
                            wellDetails.GetValueOrDefault("LocationText"),
                            wellDetails.GetValueOrDefault("Lot"),
                            wellDetails.GetValueOrDefault("FootageNSH"),
                            wellDetails.GetValueOrDefault("FootageEW"),
                            wellDetails.GetValueOrDefault("Coordinates")
                        }.Where(x => !string.IsNullOrEmpty(x))),
                        GLElevation = double.TryParse(wellDetails.GetValueOrDefault("GLElevation"), out var glElevation) ? glElevation : 0,
                        KBElevation = double.TryParse(wellDetails.GetValueOrDefault("KBElevation"), out var kbElevation) ? kbElevation : 0,
                        DFElevation = double.TryParse(wellDetails.GetValueOrDefault("DFElevation"), out var dfElevation) ? dfElevation : 0,
                        SingleMultipleCompletion = wellDetails.GetValueOrDefault("Completions") ?? string.Empty,
                        PotashWaiver = wellDetails.GetValueOrDefault("PotashWaiver") ?? string.Empty,
                        SpudDate = DateTime.TryParse(wellDetails.GetValueOrDefault("SpudDate"), out var spudDate) ? spudDate : DateTime.MinValue,
                        LastInspection = DateTime.TryParse(wellDetails.GetValueOrDefault("LastInspectionDate"), out var lastInspection) ? lastInspection : DateTime.MinValue,
                        TVD = double.TryParse(wellDetails.GetValueOrDefault("TrueVerticalDepth"), out var tvd) ? tvd : 0,
                        API = wellDetails.GetValueOrDefault("API") ?? string.Empty,
                        Latitude = double.TryParse(wellDetails.GetValueOrDefault("Latitude"), out var latitude) ? latitude : 0,
                        Longitude = double.TryParse(wellDetails.GetValueOrDefault("Longitude"), out var longitude) ? longitude : 0,
                        CRS = wellDetails.GetValueOrDefault("CRS") ?? string.Empty,
                    });
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}