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

            if (dbContext.WellDetails.Any())
            {
                _logger.LogInformation("Database has already been seeded. Skipping seed operation.");
                return;
            }

            try
            {
                await SeedDataAsync(dbContext);
                // await MultiThreadedSeedDataAsync(dbContext); // see comments in method
                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedDataAsync(ApplicationDbContext dbContext)
        {
            /* Original single-threaded version of SeedDataAsync.
             * Useful for understanding the logic before implementing multi-threading.
             */
            if (!await dbContext.WellDetails.AnyAsync())
            {
                var apiNumbers = File.ReadAllLines("apis_pythondev_test.csv")
                    .Skip(1) // skip header
                    .Select(line => line.Split(',')[0]) // get the first column (api number)
                    .ToList();

                WellDetailsParser parser = new WellDetailsParser();

                foreach (var apiNumber in apiNumbers)
                {
                    var wellDetails = await parser.ParseWellDetails(apiNumber);
                    _logger.LogInformation($"Parsed well details for API number: {apiNumber}");
                    dbContext.WellDetails.AddRange(new WellDetail
                    {
                        API = apiNumber,
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
                            wellDetails.GetValueOrDefault("Lot"),
                            wellDetails.GetValueOrDefault("FootageNSH"),
                            wellDetails.GetValueOrDefault("FootageEW"),
                        }.Where(x => !string.IsNullOrEmpty(x))),
                        GLElevation = double.TryParse(wellDetails.GetValueOrDefault("GLElevation"), out var glElevation) ? glElevation : 0,
                        KBElevation = double.TryParse(wellDetails.GetValueOrDefault("KBElevation"), out var kbElevation) ? kbElevation : 0,
                        DFElevation = double.TryParse(wellDetails.GetValueOrDefault("DFElevation"), out var dfElevation) ? dfElevation : 0,
                        SingleMultipleCompletion = wellDetails.GetValueOrDefault("Completions") ?? string.Empty,
                        PotashWaiver = wellDetails.GetValueOrDefault("PotashWaiver") ?? string.Empty,
                        SpudDate = DateTime.TryParse(wellDetails.GetValueOrDefault("SpudDate"), out var spudDate) ? spudDate : DateTime.MinValue,
                        LastInspection = DateTime.TryParse(wellDetails.GetValueOrDefault("LastInspectionDate"), out var lastInspection) ? lastInspection : DateTime.MinValue,
                        TVD = double.TryParse(wellDetails.GetValueOrDefault("TrueVerticalDepth"), out var tvd) ? tvd : 0,
                        Latitude = double.TryParse(wellDetails.GetValueOrDefault("Coordinates")?.Split(new[] { ',', ' ' })[0], out var latitude) ? latitude : 0,
                        Longitude = double.TryParse(wellDetails.GetValueOrDefault("Coordinates")?.Split(new[] { ',', ' ' })[1], out var longitude) ? longitude : 0,
                        CRS = wellDetails.GetValueOrDefault("CRS") ?? string.Empty,
                    });
                    await dbContext.SaveChangesAsync(); // parsing takes a while, so save after each one
                }
                _logger.LogInformation("Seeding completed successfully.");
            }
        }

        private async Task MultiThreadedSeedDataAsync(ApplicationDbContext dbContext)
        {
            /* Multi-threaded version of SeedDataAsync to speed up the seeding process.
             * This version parses well details in parallel and saves them in batches to the database.
             * This should significantly reduce the total time taken to seed the database.
             * 
             * Well... we can't get this to work reliably due to the site's Rate Limiting.
             * So, we are reverting to the single-threaded version for now.
             */
            if (!await dbContext.WellDetails.AnyAsync())
            {
                var apiNumbers = File.ReadAllLines("apis_pythondev_test.csv")
                    .Skip(1) // skip header
                    .Select(line => line.Split(',')[0]) // get the first column (api number)
                    .ToList();

                WellDetailsParser parser = new WellDetailsParser();
                var tasks = apiNumbers.Select(async apiNumber =>
                {
                    try
                    {
                        var wellDetails = await parser.ParseWellDetails(apiNumber);
                        _logger.LogInformation($"Parsed well details for API number: {apiNumber}");
                        return new WellDetail
                        {
                            API = apiNumber,
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
                                wellDetails.GetValueOrDefault("Lot"),
                                wellDetails.GetValueOrDefault("FootageNSH"),
                                wellDetails.GetValueOrDefault("FootageEW"),
                            }.Where(x => !string.IsNullOrEmpty(x))),
                            GLElevation = double.TryParse(wellDetails.GetValueOrDefault("GLElevation"), out var glElevation) ? glElevation : 0,
                            KBElevation = double.TryParse(wellDetails.GetValueOrDefault("KBElevation"), out var kbElevation) ? kbElevation : 0,
                            DFElevation = double.TryParse(wellDetails.GetValueOrDefault("DFElevation"), out var dfElevation) ? dfElevation : 0,
                            SingleMultipleCompletion = wellDetails.GetValueOrDefault("Completions") ?? string.Empty,
                            PotashWaiver = wellDetails.GetValueOrDefault("PotashWaiver") ?? string.Empty,
                            SpudDate = DateTime.TryParse(wellDetails.GetValueOrDefault("SpudDate"), out var spudDate) ? spudDate : DateTime.MinValue,
                            LastInspection = DateTime.TryParse(wellDetails.GetValueOrDefault("LastInspectionDate"), out var lastInspection) ? lastInspection : DateTime.MinValue,
                            TVD = double.TryParse(wellDetails.GetValueOrDefault("TrueVerticalDepth"), out var tvd) ? tvd : 0,
                            Latitude = double.TryParse(wellDetails.GetValueOrDefault("Coordinates")?.Split(new[] { ',', ' ' })[0], out var latitude) ? latitude : 0,
                            Longitude = double.TryParse(wellDetails.GetValueOrDefault("Coordinates")?.Split(new[] { ',', ' ' })[1], out var longitude) ? longitude : 0,
                            CRS = wellDetails.GetValueOrDefault("CRS") ?? string.Empty,
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error parsing well details for API number: {apiNumber}");
                        return null;
                    }
                });

                // Wait for all parsing tasks to complete
                var wellDetails = (await Task.WhenAll(tasks)).Where(x => x != null).Cast<WellDetail>().ToList();

                // Save in batches of 100 to avoid overwhelming the database
                const int batchSize = 100;
                for (int i = 0; i < wellDetails.Count; i += batchSize)
                {
                    var batch = wellDetails.Skip(i).Take(batchSize);
                    dbContext.WellDetails.AddRange(batch);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Saved batch of {batch.Count()} records ({i + batch.Count()} of {wellDetails.Count} total)");
                }

                _logger.LogInformation($"Successfully seeded {wellDetails.Count} well details");
            }
        }
    }
}