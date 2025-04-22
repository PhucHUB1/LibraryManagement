using LibraryManagement.Models.ViewModels;

namespace LibraryManagement.Services.Implement;

public interface IStatisticsService
{
    DashboardViewModel GetDashboardData();
    List<BookBorrowViewModel> GetOverdueBooks();
    object GenerateReport(string reportType, DateTime? startDate, DateTime? endDate);
}