using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; } // Optional field
        public string? Phone { get; set; } // Optional field
        public string? Address { get; set; } // Optional field
    }
}
