namespace Domain.Entities;

public class Course {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    public List<Student> Students { get; set; } = new();
}