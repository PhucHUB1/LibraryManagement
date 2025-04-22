using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LibraryManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime MemberSince { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<BookBorrow> BookBorrows { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}