using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class BookBorrow
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        
        public DateTime DueDate { get; set; }
        
        public DateTime? ReturnDate { get; set; }
        
        public bool IsReturned => ReturnDate.HasValue;
        
        // Calculate if the book is overdue
        public bool IsOverdue => !ReturnDate.HasValue && DateTime.Now > DueDate;
        
        // Foreign keys
        public int BookId { get; set; }
        public string UserId { get; set; }
        
        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}