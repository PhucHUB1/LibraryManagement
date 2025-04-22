namespace LibraryManagement.Models.ViewModels.Book;

public class BookSearchViewModel
{
    public string SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? AuthorId { get; set; }
    public List<BookDetailViewModel> Books { get; set; } = new List<BookDetailViewModel>();
}