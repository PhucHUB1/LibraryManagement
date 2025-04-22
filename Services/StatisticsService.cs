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
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBorrowService _borrowService;
        private readonly IReservationService _reservationService;

        public StatisticsService(ApplicationDbContext context, IBorrowService borrowService, IReservationService reservationService)
        {
            _context = context;
            _borrowService = borrowService;
            _reservationService = reservationService;
        }

        public DashboardViewModel GetDashboardData()
        {
            var totalBooks = _context.Books.Sum(b => b.CopiesOwned);
            var availableBooks = _context.Books.Sum(b => b.AvailableCopies);
            var totalMembers = _context.Users.Count();
            var totalBorrows = _context.BookBorrows.Count();
            var totalReservations = _context.Reservations.Count(r => r.IsActive);
            var overdueBooks = _context.BookBorrows.Count(b => !b.ReturnDate.HasValue && b.DueDate < DateTime.Now);

            return new DashboardViewModel
            {
                TotalBooks = totalBooks,
                AvailableBooks = availableBooks,
                TotalMembers = totalMembers,
                TotalBorrows = totalBorrows,
                TotalReservations = totalReservations,
                OverdueBooks = overdueBooks,
                RecentBorrows = GetRecentBorrows(5),
                RecentReservations = GetRecentReservations(5)
            };
        }

        private List<BookBorrowViewModel> GetRecentBorrows(int count)
        {
            return _context.BookBorrows
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderByDescending(b => b.BorrowDate)
                .Take(count)
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

        private List<ReservationViewModel> GetRecentReservations(int count)
        {
            return _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.ReservationDate)
                .Take(count)
                .Select(r => new ReservationViewModel
                {
                    Id = r.Id,
                    BookId = r.BookId,
                    BookTitle = r.Book.Title,
                    BookCoverUrl = r.Book.CoverImageUrl,
                    UserId = r.UserId,
                    UserName = $"{r.User.FirstName} {r.User.LastName}",
                    ReservationDate = r.ReservationDate,
                    ExpiryDate = r.ExpiryDate,
                    IsActive = r.IsActive
                })
                .ToList();
        }

        public List<BookBorrowViewModel> GetOverdueBooks()
        {
            return _context.BookBorrows
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => !b.ReturnDate.HasValue && b.DueDate < DateTime.Now)
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

        public object GenerateReport(string reportType, DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            switch (reportType.ToLower())
            {
                case "borrows":
                    return GenerateBorrowsReport(startDate.Value, endDate.Value);
                case "returns":
                    return GenerateReturnsReport(startDate.Value, endDate.Value);
                case "popular":
                    return GeneratePopularBooksReport(startDate.Value, endDate.Value);
                case "users":
                    return GenerateActiveUsersReport(startDate.Value, endDate.Value);
                default:
                    throw new ArgumentException("Loại báo cáo không hợp lệ");
            }
        }

        private object GenerateBorrowsReport(DateTime startDate, DateTime endDate)
        {
            var borrows = _context.BookBorrows
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => b.BorrowDate >= startDate && b.BorrowDate <= endDate)
                .GroupBy(b => new { Year = b.BorrowDate.Year, Month = b.BorrowDate.Month, Day = b.BorrowDate.Day })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    Count = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToList();

            return new
            {
                ReportType = "Báo cáo mượn sách",
                StartDate = startDate,
                EndDate = endDate,
                TotalCount = borrows.Sum(b => b.Count),
                DailyData = borrows
            };
        }

        private object GenerateReturnsReport(DateTime startDate, DateTime endDate)
        {
            var returns = _context.BookBorrows
                .Include(b => b.Book)
                .Where(b => b.ReturnDate.HasValue && b.ReturnDate >= startDate && b.ReturnDate <= endDate)
                .GroupBy(b => new { Year = b.ReturnDate.Value.Year, Month = b.ReturnDate.Value.Month, Day = b.ReturnDate.Value.Day })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                    Count = g.Count(),
                    OnTimeCount = g.Count(b => b.ReturnDate <= b.DueDate),
                    LateCount = g.Count(b => b.ReturnDate > b.DueDate),
                    AverageDaysLate = g.Where(b => b.ReturnDate > b.DueDate)
                        .Select(b => (b.ReturnDate.Value - b.DueDate).Days)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderBy(r => r.Date)
                .ToList();

            return new
            {
                ReportType = "Báo cáo trả sách",
                StartDate = startDate,
                EndDate = endDate,
                TotalCount = returns.Sum(r => r.Count),
                OnTimeCount = returns.Sum(r => r.OnTimeCount),
                LateCount = returns.Sum(r => r.LateCount),
                LatePercentage = returns.Sum(r => r.Count) > 0 
                    ? (double)returns.Sum(r => r.LateCount) / returns.Sum(r => r.Count) * 100 
                    : 0,
                AverageDaysLate = returns.Where(r => r.LateCount > 0)
                    .Select(r => r.AverageDaysLate)
                    .DefaultIfEmpty(0)
                    .Average(),
                DailyData = returns
            };
        }

        private object GeneratePopularBooksReport(DateTime startDate, DateTime endDate)
        {
            var popularBooks = _context.BookBorrows
                .Include(b => b.Book)
                .Where(b => b.BorrowDate >= startDate && b.BorrowDate <= endDate)
                .GroupBy(b => new { b.BookId, Title = b.Book.Title, Author = b.Book.Author, Category = b.Book.Category.Name })
                .Select(g => new
                {
                    BookId = g.Key.BookId,
                    Title = g.Key.Title,
                    Author = g.Key.Author,
                    Category = g.Key.Category,
                    BorrowCount = g.Count(),
                    AverageBorrowDays = g.Where(b => b.ReturnDate.HasValue)
                        .Select(b => (b.ReturnDate.Value - b.BorrowDate).Days)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderByDescending(b => b.BorrowCount)
                .ToList();

            var categoryStats = popularBooks
                .GroupBy(b => b.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    BorrowCount = g.Sum(b => b.BorrowCount),
                    BookCount = g.Count(),
                    AverageBorrowsPerBook = (double)g.Sum(b => b.BorrowCount) / g.Count()
                })
                .OrderByDescending(c => c.BorrowCount)
                .ToList();

            return new
            {
                ReportType = "Báo cáo sách phổ biến",
                StartDate = startDate,
                EndDate = endDate,
                TotalBorrows = popularBooks.Sum(b => b.BorrowCount),
                TopBooks = popularBooks.Take(10).ToList(),
                CategoryStatistics = categoryStats
            };
        }

        private object GenerateActiveUsersReport(DateTime startDate, DateTime endDate)
        {
            var activeUsers = _context.BookBorrows
                .Include(b => b.User)
                .Where(b => b.BorrowDate >= startDate && b.BorrowDate <= endDate)
                .GroupBy(b => new { b.UserId, UserName = $"{b.User.FirstName} {b.User.LastName}", Email = b.User.Email })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName,
                    Email = g.Key.Email,
                    BorrowCount = g.Count(),
                    ReturnedCount = g.Count(b => b.ReturnDate.HasValue),
                    LateReturnCount = g.Count(b => b.ReturnDate.HasValue && b.ReturnDate > b.DueDate),
                    CurrentlyBorrowed = g.Count(b => !b.ReturnDate.HasValue),
                    CurrentlyOverdue = g.Count(b => !b.ReturnDate.HasValue && b.DueDate < DateTime.Now)
                })
                .OrderByDescending(u => u.BorrowCount)
                .ToList();

            return new
            {
                ReportType = "Báo cáo độc giả hoạt động",
                StartDate = startDate,
                EndDate = endDate,
                TotalActiveUsers = activeUsers.Count,
                TotalBorrows = activeUsers.Sum(u => u.BorrowCount),
                AverageBorrowsPerUser = activeUsers.Count > 0 
                    ? (double)activeUsers.Sum(u => u.BorrowCount) / activeUsers.Count 
                    : 0,
                CurrentlyBorrowed = activeUsers.Sum(u => u.CurrentlyBorrowed),
                CurrentlyOverdue = activeUsers.Sum(u => u.CurrentlyOverdue),
                TopUsers = activeUsers.Take(10).ToList()
            };
        }
    }
}