using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class CollegeDbContext(DbContextOptions<CollegeDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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


        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(g => new { g.Id, g.TeacherId })
                .IsUnique();

            entity.HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasOne(l => l.Student)
                .WithMany(s => s.Loans)
                .HasForeignKey(l => l.StudentId);

            entity.HasOne(l => l.Book)
                    .WithMany(b => b.Loans)
                    .HasForeignKey(l => l.BookId);

            entity.Property(l => l.BorrowedAt).IsRequired();
        });
  
        base.OnModelCreating(modelBuilder);
    }
}