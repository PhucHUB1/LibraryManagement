using LibraryManagement.Models.ViewModels;

namespace LibraryManagement.Services.Implement
{
    public interface IBorrowService
    {
        List<BookBorrowViewModel> GetAllBorrows();
        List<BookBorrowViewModel> GetUserBorrows(string userId);
        BookBorrowViewModel GetBorrowById(int id);
        void ReturnBook(int borrowId);
    }
}