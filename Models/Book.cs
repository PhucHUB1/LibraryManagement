using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        
        [StringLength(20)]
        public string ISBN { get; set; }
        
        public int PublicationYear { get; set; }
        
        [Required]
        public int CopiesOwned { get; set; }
        
        public string Description { get; set; }
        
        public string CoverImageUrl { get; set; }
        
        public DateTime DateAdded { get; set; } = DateTime.Now;
        
        // Foreign keys
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        
        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual Author Author { get; set; }
        public virtual ICollection<BookBorrow> BookBorrows { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        
        public int AvailableCopies { get; set; }
    }
}