using LibraryManagement.Models.ViewModels;
using LibraryManagement.Services.Implement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LibraryManagement.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class AdminController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IBorrowService _borrowService;
        private readonly IReservationService _reservationService;
        private readonly IUserService _userService;

        public AdminController(
            IStatisticsService statisticsService,
            IBorrowService borrowService,
            IReservationService reservationService,
            IUserService userService)
        {
            _statisticsService = statisticsService;
            _borrowService = borrowService;
            _reservationService = reservationService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var dashboardData = _statisticsService.GetDashboardData();
            return View(dashboardData);
        }

        public IActionResult BorrowsList()
        {
            var borrows = _borrowService.GetAllBorrows();
            return View(borrows);
        }

        public IActionResult ReservationsList()
        {
            var reservations = _reservationService.GetAllReservations();
            return View(reservations);
        }

        public IActionResult UsersList()
        {
            var users = _userService.GetAllUsers();
            return View(users);
        }

        public IActionResult OverdueBooks()
        {
            var overdueBooks = _statisticsService.GetOverdueBooks();
            return View(overdueBooks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReturnBook(int id)
        {
            try
            {
                _borrowService.ReturnBook(id);
                return RedirectToAction(nameof(BorrowsList));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(BorrowsList));
            }
        }

        public IActionResult GenerateReport(string reportType, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var report = _statisticsService.GenerateReport(reportType, startDate, endDate);
                return View("Report", report);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}