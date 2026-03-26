namespace Domain.Entities;

public class Teacher {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Course> Courses { get; set; } = new();
}