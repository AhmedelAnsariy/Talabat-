using System.ComponentModel.DataAnnotations;

namespace Talabat.APIS.DTO
{
    public class RegisterDto
    {


        [Required]
        public string DisplayName { get; set; }



        [Required]
        [Phone]
        public string phone { get; set; }


        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
      ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }


        [Required]
        public string Email { get; set; }






    }
}
