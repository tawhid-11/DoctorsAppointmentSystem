using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    
    [Display(Name = "Full Name")]
    public required string FullName { get; set; }

    [Required]
    [Range(1, 120)]
    public int Age { get; set; }

    
    [Phone]
    [Display(Name = "Contact Number")]
    public required string Contact { get; set; }

    
    [EmailAddress]
    public required string Email { get; set; }

    
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }

    
    [Display(Name = "Register As")]
    public required string Role { get; set; } // "Patient" or "Doctor"
}
