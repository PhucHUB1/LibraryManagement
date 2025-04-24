using LibraryManagement.Models.ViewModels.Book;
using System.Collections.Generic;

namespace LibraryManagement.Services.Implement
{
    public interface IBookService
    {
        List<BookDetailViewModel> GetAllBooks();
        List<BookDetailViewModel> GetLatestBooks(int count);
        List<BookDetailViewModel> GetPopularBooks(int count);
        List<BookDetailViewModel> GetFilteredBooks(string searchTerm, int? categoryId, int? authorId);
        BookDetailViewModel GetBookById(int id);
        BookViewModel GetBookForEdit(int id);
        void AddBook(BookViewModel model);
        void UpdateBook(BookViewModel model);
        void DeleteBook(int id);
        void BorrowBook(int bookId, string userId);
        void ReserveBook(int bookId, string userId);
    }
}