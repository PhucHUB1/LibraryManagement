using System.Collections.Generic;

namespace LibraryManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int TotalBorrows { get; set; }
        public int TotalReservations { get; set; }
        public int OverdueBooks { get; set; }
        public int AvailableBooks { get; set; }
        public List<BookBorrowViewModel> RecentBorrows { get; set; } = new List<BookBorrowViewModel>();
        public List<ReservationViewModel> RecentReservations { get; set; } = new List<ReservationViewModel>();
    }
}