using DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                      ?? "Host=localhost;Port=5432;Username=postgres;Password=123;Database=questly";

        optionsBuilder.UseNpgsql(connStr);

        return new DatabaseContext(optionsBuilder.Options);
    }
}