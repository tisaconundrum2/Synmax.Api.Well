using Microsoft.EntityFrameworkCore;
using Synmax.Api.Well.Models;

namespace Synmax.Api.Well.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<WellDetail> WellDetails { get; set; }
    }
}