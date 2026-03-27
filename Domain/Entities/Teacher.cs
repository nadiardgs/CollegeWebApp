namespace Domain.Entities;

public class Teacher 
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}