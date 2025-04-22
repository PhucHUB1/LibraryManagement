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
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ReservationViewModel> GetAllReservations()
        {
            return _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
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

        public List<ReservationViewModel> GetUserReservations(string userId)
        {
            return _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
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

        public ReservationViewModel GetReservationById(int id)
        {
            return _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.Id == id)
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
                .FirstOrDefault();
        }

        public void CancelReservation(int reservationId)
        {
            var reservation = _context.Reservations.Find(reservationId);
            if (reservation != null && reservation.IsActive)
            {
                reservation.IsActive = false;
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Đặt chỗ không hợp lệ hoặc đã bị hủy");
            }
        }
    }
}