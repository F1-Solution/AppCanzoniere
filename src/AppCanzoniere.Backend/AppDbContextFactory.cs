using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AppCanzoniere.Backend;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=appcanzoniere_backend.db",
            b => b.MigrationsAssembly("AppCanzoniere.Backend"));

        return new AppDbContext(optionsBuilder.Options);
    }
}
