using LibraryManagement.Models.ViewModels;

namespace LibraryManagement.Services.Implement;

public interface IUserService
{
    List<UserViewModel> GetAllUsers();
    UserViewModel GetUserById(string id);
}