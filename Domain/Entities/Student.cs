namespace Domain.Entities;

public class Student 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public List<Loan> Loans { get; set; } = [];
}