using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace odev1.Data
{
    public class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // appsettings.json YOK
            // configuration YOK
            // direkt connection string

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=odev1db;Username=postgres;Password=1905"
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}


