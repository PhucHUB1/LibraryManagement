using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Author
    {
        public int Id { get; set; }
      
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        public string Biography { get; set; }
        
        // Navigation property
        public virtual ICollection<Book> Books { get; set; }
        
        // Full name calculated property
        public string FullName => $"{FirstName} {LastName}";
    }
}