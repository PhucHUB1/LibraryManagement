using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.ViewModels.Book
{
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề sách")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "ISBN")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập năm xuất bản")]
        [Display(Name = "Năm xuất bản")]
        [Range(1000, 3000, ErrorMessage = "Năm xuất bản không hợp lệ")]
        public int PublicationYear { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sách")]
        [Display(Name = "Số lượng")]
        [Range(1, 1000, ErrorMessage = "Số lượng sách phải từ 1 đến 1000")]
        public int CopiesOwned { get; set; }

        [Display(Name = "Mô tả")] public string Description { get; set; }

        [Display(Name = "Ảnh bìa")] public IFormFile CoverImage { get; set; }

        public string ExistingCoverImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        [Display(Name = "Thể loại")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tác giả")]
        [Display(Name = "Tác giả")]
        public int AuthorId { get; set; }

      
    }
}