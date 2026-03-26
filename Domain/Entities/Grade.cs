namespace Domain.Entities;

public class Grade {
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public Guid CourseId { get; set; }
}