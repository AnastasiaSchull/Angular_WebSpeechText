using System.ComponentModel.DataAnnotations;

namespace Angular_WebSpeechText.Models
{
    public class RegisterModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)] //тип данных для email                                          
        [RegularExpression(@"^[\w.-]+@([\w-]+\.)+[\w]{2,4}$", ErrorMessage = "Invalid email format")]

        public string? Login { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Password must be at least 3 characters long")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string? PasswordConfirm { get; set; }
    }
}
