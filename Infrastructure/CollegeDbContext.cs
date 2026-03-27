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
        modelBuilder.UseIdentityColumns();
        
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasIndex(g => new { g.StudentId, g.CourseId })
                .IsUnique();

            entity.Property(g => g.Value)
                .HasPrecision(5, 2);
        });

        modelBuilder.Entity<Student>()
            .HasMany(s => s.Courses)
            .WithMany(c => c.Students);
        
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(t => t.Courses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
            
        base.OnModelCreating(modelBuilder);
    }
}