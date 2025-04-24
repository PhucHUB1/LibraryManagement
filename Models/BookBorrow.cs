using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class BookBorrow
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        
        [Required]
        public DateTime DueDate { get; set; }
        
        public DateTime? ReturnDate { get; set; }
        
        // Read-only properties
        public bool IsReturned => ReturnDate.HasValue;
        
        // Calculate if the book is overdue
        public bool IsOverdue => !ReturnDate.HasValue && DateTime.Now > DueDate;
        
        // Days overdue (0 if not overdue)
        public int DaysOverdue => IsOverdue ? (int)(DateTime.Now - DueDate).TotalDays : 0;
        
        // Foreign keys
        [Required]
        public int BookId { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}