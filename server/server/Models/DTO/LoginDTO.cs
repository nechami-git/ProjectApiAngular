using System.ComponentModel.DataAnnotations;

namespace server.Models.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "פורמט אימייל לא תקין")]
        public string? Email { get; set; }



        [Required(ErrorMessage = "סיסמה היא שדה חובה")]
        public string? Password { get; set; }
    }
}
