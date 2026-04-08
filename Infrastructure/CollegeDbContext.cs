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
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityColumns();

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.Property(g => g.Value)
                .HasPrecision(5, 2);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.Name)
                .IsUnique();
        });

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

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            entity                
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            entity
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}