namespace LibraryManagement.Models.ViewModels;

public class BookBorrowViewModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; }
    public string BookCoverUrl { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue => !ReturnDate.HasValue && DateTime.Now > DueDate;
    public string UserName { get; set; }
    public string UserId { get; set; }
}