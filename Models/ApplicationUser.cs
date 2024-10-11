using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LiberaryManagmentSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime MembershipDate { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }
        public virtual IEnumerable<Checkout> Checkouts { get; set; } = new List<Checkout>();
    }
}
