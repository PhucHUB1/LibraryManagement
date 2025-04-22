using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.ViewModels;

public class AuthorViewModel
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "Vui lòng nhập tên")]
    [Display(Name = "Tên")]
    public string FirstName { get; set; }
        
    [Required(ErrorMessage = "Vui lòng nhập họ")]
    [Display(Name = "Họ")]
    public string LastName { get; set; }
        
    [Display(Name = "Tiểu sử")]
    public string Biography { get; set; }
        
    public string FullName => $"{FirstName} {LastName}";
        
    public int BookCount { get; set; }
}