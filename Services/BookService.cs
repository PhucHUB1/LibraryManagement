using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels;
using LibraryManagement.Models.ViewModels.Book;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagement.Services.Implement;

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
                    AvailableCopies = b.AvailableCopies,
                    CopiesOwned = b.CopiesOwned
                })
                .ToList();
        }

        public List<BookDetailViewModel> GetLatestBooks(int count)
        {
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
                    AvailableCopies = b.AvailableCopies,
                    CopiesOwned = b.CopiesOwned
                })
                .ToList();
        }

        public List<BookDetailViewModel> GetPopularBooks(int count)
        {
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
                    AvailableCopies = b.AvailableCopies,
                    CopiesOwned = b.CopiesOwned
                })
                .ToList();
        }

        public List<BookDetailViewModel> GetFilteredBooks(string searchTerm, int? categoryId, int? authorId)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Description.Contains(searchTerm) || b.ISBN.Contains(searchTerm));
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
                AvailableCopies = b.AvailableCopies,
                CopiesOwned = b.CopiesOwned
            }).ToList();
        }

        public BookDetailViewModel GetBookById(int id)
        {
            return _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
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
                    AvailableCopies = b.AvailableCopies,
                    CopiesOwned = b.CopiesOwned
                })
                .FirstOrDefault();
        }

        public BookViewModel GetBookForEdit(int id)
        {
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
            var book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                ISBN = model.ISBN,
                PublicationYear = model.PublicationYear,
                CoverImageUrl = model.ExistingCoverImageUrl,
                AuthorId = model.AuthorId,
                CategoryId = model.CategoryId,
                CopiesOwned = model.CopiesOwned
            };

            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public void UpdateBook(BookViewModel model)
        {
            var book = _context.Books.Find(model.Id);
            if (book != null)
            {
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
        }

        public void DeleteBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
        }

        public void BorrowBook(int bookId, string userId)
        {
            var book = _context.Books.Find(bookId);
            if (book != null && book.AvailableCopies > 0)
            {
                var borrow = new BookBorrow
                {
                    BookId = bookId,
                    UserId = userId,
                    BorrowDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14)
                };
                
                _context.BookBorrows.Add(borrow);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Sách không có sẵn để mượn");
            }
        }

        public void ReserveBook(int bookId, string userId)
        {
            var book = _context.Books.Find(bookId);
            if (book != null)
            {
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
            }
            else
            {
                throw new InvalidOperationException("Không tìm thấy sách");
            }
        }
    }
}