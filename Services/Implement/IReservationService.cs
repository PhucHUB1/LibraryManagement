using LibraryManagement.Models.ViewModels;

namespace LibraryManagement.Services.Implement
{
    public interface IReservationService
    {
        List<ReservationViewModel> GetAllReservations();
        List<ReservationViewModel> GetUserReservations(string userId);
        ReservationViewModel GetReservationById(int id);
        void CancelReservation(int reservationId);
    }
}