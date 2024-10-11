using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var errors = new List<string>();

        if (value is not string password || string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required.");
            return new ValidationResult(string.Join(", ", errors));
        }

        if (password.Length < 6)
            errors.Add("Password must be at least 6 characters long.");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            errors.Add("Password must contain at least one uppercase letter.");

        if (!Regex.IsMatch(password, @"[a-z]"))
            errors.Add("Password must contain at least one lowercase letter.");

        if (!Regex.IsMatch(password, @"\d"))
            errors.Add("Password must contain at least one number.");

        if (!Regex.IsMatch(password, @"[!@#$%_^&*]"))
            errors.Add("Password must contain at least one special character.");

        return errors.Count > 0 ? new ValidationResult(string.Join(", ", errors)) : ValidationResult.Success;
    }
}
