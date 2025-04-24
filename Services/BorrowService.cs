using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels;
using LibraryManagement.Services.Implement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly ApplicationDbContext _context;

        public BorrowService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<BookBorrowViewModel> GetAllBorrows()
        {
            return _context.BookBorrows
                .Include(b => b.Book)
                .Include(b => b.User)
                .Select(b => new BookBorrowViewModel
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    BookCoverUrl = b.Book.CoverImageUrl,
                    UserId = b.UserId,
                    UserName = $"{b.User.FirstName} {b.User.LastName}",
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate
                })
                .ToList();
        }

        public List<BookBorrowViewModel> GetUserBorrows(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            return _context.BookBorrows
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .Select(b => new BookBorrowViewModel
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    BookCoverUrl = b.Book.CoverImageUrl,
                    UserId = b.UserId,
                    UserName = $"{b.User.FirstName} {b.User.LastName}",
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate
                })
                .ToList();
        }

        public BookBorrowViewModel GetBorrowById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid borrow ID", nameof(id));

            return _context.BookBorrows
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => b.Id == id)
                .Select(b => new BookBorrowViewModel
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    BookTitle = b.Book.Title,
                    BookCoverUrl = b.Book.CoverImageUrl,
                    UserId = b.UserId,
                    UserName = $"{b.User.FirstName} {b.User.LastName}",
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate
                })
                .FirstOrDefault();
        }

        public void ReturnBook(int borrowId)
        {
            if (borrowId <= 0)
                throw new ArgumentException("Invalid borrow ID", nameof(borrowId));

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var borrow = _context.BookBorrows.Find(borrowId);
                    if (borrow == null)
                        throw new InvalidOperationException($"Borrow record with ID {borrowId} not found");

                    if (borrow.ReturnDate.HasValue)
                        throw new InvalidOperationException("This book has already been returned");

                    borrow.ReturnDate = DateTime.Now;
                    _context.SaveChanges();

                    // Check if there are any active reservations for this book
                    var activeReservations = _context.Reservations
                        .Where(r => r.BookId == borrow.BookId && r.IsActive)
                        .OrderBy(r => r.ReservationDate)
                        .ToList();

                    // If needed, you could implement notification logic here
                    // for the first person in the reservation queue

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