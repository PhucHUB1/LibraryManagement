using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.ViewModels;

public class CategoryViewModel
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "Vui lòng nhập tên thể loại")]
    [Display(Name = "Tên thể loại")]
    public string Name { get; set; }
        
    [Display(Name = "Mô tả")]
    public string Description { get; set; }
        
    public int BookCount { get; set; }
}