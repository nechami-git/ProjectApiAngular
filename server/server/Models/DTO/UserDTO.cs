using System.ComponentModel.DataAnnotations;

namespace server.Models.DTO
{
    public class UserDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }


        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "פורמט אימייל לא תקין")]
        public string? Email { get; set; }
        public string? Phone { get; set; }

    }
}
