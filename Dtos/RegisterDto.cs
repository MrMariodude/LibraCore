using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace LiberaryManagmentSystem.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        [PasswordValidation]
        public required string Password { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Membership date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [BindNever]
        public DateTime MembershipDate { get; } = DateTime.Now;
        public required string Role { get; set; } = "User";

    }
}
