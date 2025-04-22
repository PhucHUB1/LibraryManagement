namespace LibraryManagement.Models.ViewModels.Book;

public class BookDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public int PublicationYear { get; set; }
    public string Description { get; set; }
    public string CoverImageUrl { get; set; }
    public string CategoryName { get; set; }
    public string AuthorName { get; set; }
    public int CopiesOwned { get; set; }
    public int AvailableCopies { get; set; }
    public bool IsAvailable => AvailableCopies > 0;
}