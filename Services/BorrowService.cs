using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagement.Services.Implement;

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
            var borrow = _context.BookBorrows.Find(borrowId);
            if (borrow != null && !borrow.ReturnDate.HasValue)
            {
                borrow.ReturnDate = DateTime.Now;
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Phiếu mượn không hợp lệ hoặc sách đã được trả");
            }
        }
    }
}