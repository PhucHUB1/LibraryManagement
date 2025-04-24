using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels;
using LibraryManagement.Models.ViewModels.Book;
using LibraryManagement.Services.Implement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<BookDetailViewModel> GetAllBooks()
        {
            return _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Select(b => new BookDetailViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    ISBN = b.ISBN,
                    PublicationYear = b.PublicationYear,
                    CoverImageUrl = b.CoverImageUrl,
                    AuthorName = $"{b.Author.FirstName} {b.Author.LastName}",
                    CategoryName = b.Category.Name,
                    CopiesOwned = b.CopiesOwned,
                    AvailableCopies = b.CopiesOwned - b.BookBorrows.Count(bb => bb.ReturnDate == null)
                })
                .ToList();
        }

        public List<BookDetailViewModel> GetLatestBooks(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than 0", nameof(count));

            return _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .OrderByDescending(b => b.PublicationYear)
                .Take(count)
                .Select(b => new BookDetailViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    ISBN = b.ISBN,
                    PublicationYear = b.PublicationYear,
                    CoverImageUrl = b.CoverImageUrl,
                    AuthorName = $"{b.Author.FirstName} {b.Author.LastName}",
                    CategoryName = b.Category.Name,
                    CopiesOwned = b.CopiesOwned,
                    AvailableCopies = b.CopiesOwned - b.BookBorrows.Count(bb => bb.ReturnDate == null)
                })
                .ToList();
        }

        public List<BookDetailViewModel> GetPopularBooks(int count)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than 0", nameof(count));

            return _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.BookBorrows)
                .OrderByDescending(b => b.BookBorrows.Count)
                .Take(count)
                .Select(b => new BookDetailViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    ISBN = b.ISBN,
                    PublicationYear = b.PublicationYear,
                    CoverImageUrl = b.CoverImageUrl,
                    AuthorName = $"{b.Author.FirstName} {b.Author.LastName}",
                    CategoryName = b.Category.Name,
                    CopiesOwned = b.CopiesOwned,
                    AvailableCopies = b.CopiesOwned - b.BookBorrows.Count(bb => bb.ReturnDate == null)
                })
                .ToList();
        }

        public List<BookDetailViewModel> GetFilteredBooks(string searchTerm, int? categoryId, int? authorId)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.BookBorrows)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b => 
                    b.Title.ToLower().Contains(searchTerm) || 
                    (b.Description != null && b.Description.ToLower().Contains(searchTerm)) || 
                    (b.ISBN != null && b.ISBN.ToLower().Contains(searchTerm)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            if (authorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == authorId.Value);
            }

            return query.Select(b => new BookDetailViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                ISBN = b.ISBN,
                PublicationYear = b.PublicationYear,
                CoverImageUrl = b.CoverImageUrl,
                AuthorName = $"{b.Author.FirstName} {b.Author.LastName}",
                CategoryName = b.Category.Name,
                CopiesOwned = b.CopiesOwned,
                AvailableCopies = b.CopiesOwned - b.BookBorrows.Count(bb => bb.ReturnDate == null)
            }).ToList();
        }

        public BookDetailViewModel GetBookById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid book ID", nameof(id));

            return _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.BookBorrows)
                .Where(b => b.Id == id)
                .Select(b => new BookDetailViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    ISBN = b.ISBN,
                    PublicationYear = b.PublicationYear,
                    CoverImageUrl = b.CoverImageUrl,
                    AuthorName = $"{b.Author.FirstName} {b.Author.LastName}",
                    CategoryName = b.Category.Name,
                    CopiesOwned = b.CopiesOwned,
                    AvailableCopies = b.CopiesOwned - b.BookBorrows.Count(bb => bb.ReturnDate == null)
                })
                .FirstOrDefault();
        }

        public BookViewModel GetBookForEdit(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid book ID", nameof(id));

            var book = _context.Books
                .FirstOrDefault(b => b.Id == id);
        
            if (book == null)
            {
                return null;
            }
    
            return new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                ISBN = book.ISBN,
                PublicationYear = book.PublicationYear,
                ExistingCoverImageUrl = book.CoverImageUrl, 
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                CopiesOwned = book.CopiesOwned
            };
        }

        public void AddBook(BookViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Book model cannot be null");

            var book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                ISBN = model.ISBN,
                PublicationYear = model.PublicationYear,
                CoverImageUrl = model.ExistingCoverImageUrl,
                AuthorId = model.AuthorId,
                CategoryId = model.CategoryId,
                CopiesOwned = model.CopiesOwned,
                DateAdded = DateTime.Now
            };

            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public void UpdateBook(BookViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Book model cannot be null");

            var book = _context.Books.Find(model.Id);
            if (book == null)
                throw new InvalidOperationException($"Book with ID {model.Id} not found");

            book.Title = model.Title;
            book.Description = model.Description;
            book.ISBN = model.ISBN;
            book.PublicationYear = model.PublicationYear;
            book.CoverImageUrl = model.ExistingCoverImageUrl;
            book.AuthorId = model.AuthorId;
            book.CategoryId = model.CategoryId;
            book.CopiesOwned = model.CopiesOwned;

            _context.Update(book);
            _context.SaveChanges();
        }

        public void DeleteBook(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid book ID", nameof(id));

            var book = _context.Books.Find(id);
            if (book == null)
                throw new InvalidOperationException($"Book with ID {id} not found");

            // Check if the book has active borrows or reservations
            bool hasActiveBorrows = _context.BookBorrows.Any(b => b.BookId == id && b.ReturnDate == null);
            if (hasActiveBorrows)
                throw new InvalidOperationException("Cannot delete book that has active borrows");

            bool hasActiveReservations = _context.Reservations.Any(r => r.BookId == id && r.IsActive);
            if (hasActiveReservations)
                throw new InvalidOperationException("Cannot delete book that has active reservations");

            _context.Books.Remove(book);
            _context.SaveChanges();
        }

        public void BorrowBook(int bookId, string userId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID", nameof(bookId));

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            // Use a transaction to ensure atomicity
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Get the book with a lock to prevent concurrency issues
                    var book = _context.Books
                        .Include(b => b.BookBorrows.Where(bb => bb.ReturnDate == null))
                        .FirstOrDefault(b => b.Id == bookId);

                    if (book == null)
                        throw new InvalidOperationException($"Book with ID {bookId} not found");

                    // Calculate available copies
                    int availableCopies = book.CopiesOwned - book.BookBorrows.Count;

                    if (availableCopies <= 0)
                        throw new InvalidOperationException("No copies available for borrowing");

                   
                    bool alreadyBorrowed = _context.BookBorrows
                        .Any(b => b.BookId == bookId && b.UserId == userId && b.ReturnDate == null);

                    if (alreadyBorrowed)
                        throw new InvalidOperationException("User already has this book borrowed");

                    var borrow = new BookBorrow
                    {
                        BookId = bookId,
                        UserId = userId,
                        BorrowDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(14)
                    };
                
                    _context.BookBorrows.Add(borrow);
                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void ReserveBook(int bookId, string userId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Invalid book ID", nameof(bookId));

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var book = _context.Books.Find(bookId);
                    if (book == null)
                        throw new InvalidOperationException($"Book with ID {bookId} not found");

                 
                    bool alreadyReserved = _context.Reservations
                        .Any(r => r.BookId == bookId && r.UserId == userId && r.IsActive);

                    if (alreadyReserved)
                        throw new InvalidOperationException("User already has this book reserved");

                    var reservation = new Reservation
                    {
                        BookId = bookId,
                        UserId = userId,
                        ReservationDate = DateTime.Now,
                        ExpiryDate = DateTime.Now.AddDays(7),
                        IsActive = true
                    };

                    _context.Reservations.Add(reservation);
                    _context.SaveChanges();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}