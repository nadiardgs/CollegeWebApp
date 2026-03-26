namespace Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class CollegeDbContextFactory : IDesignTimeDbContextFactory<CollegeDbContext>
{
    public CollegeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CollegeDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Database=college_db;Username=postgres;Password=yourpassword");

        return new CollegeDbContext(optionsBuilder.Options);
    }
}