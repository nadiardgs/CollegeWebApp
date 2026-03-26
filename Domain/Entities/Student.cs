namespace Domain.Entities;

public class Student {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Course> Courses { get; set; } = new();
    public List<Grade> Grades { get; set; } = new();
}