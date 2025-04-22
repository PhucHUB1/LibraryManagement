using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime ReservationDate { get; set; } = DateTime.Now;
        
        public DateTime ExpiryDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Foreign keys
        public int BookId { get; set; }
        public string UserId { get; set; }
        
        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}