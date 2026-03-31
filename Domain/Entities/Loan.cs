namespace Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int StudentId { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }

    public Book Book { get; set; } = null!;
    public Student Student { get; set; } = null!;
}