using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RegisterDto
    {
        [Required]
        [MinLength(2)]
        public string Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4)]
        public string Password { get; set; }
    }
}