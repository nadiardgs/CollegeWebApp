namespace Domain.Entities;

public class Grade
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    
    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;
}