using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        //No data validation attributes for password,
        //as it will be validated in the using Identity
        public string Password { get; set; } = "";
    }
}
