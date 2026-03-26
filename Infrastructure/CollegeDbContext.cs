using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class CollegeDbContext(DbContextOptions<CollegeDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Precise decimal for grades
        modelBuilder.Entity<Grade>()
            .Property(g => g.Value)
            .HasPrecision(5, 2);

        // Define Many-to-Many Student <-> Course
        modelBuilder.Entity<Student>()
            .HasMany(s => s.Courses)
            .WithMany(c => c.Students);
            
        base.OnModelCreating(modelBuilder);
    }
}