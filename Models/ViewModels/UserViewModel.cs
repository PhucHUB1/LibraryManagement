namespace LibraryManagement.Models.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime MemberSince { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}