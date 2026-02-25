using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Sockets;

namespace server.Models
{
    public enum Role
    {
        Manager,
        User
    }
    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [MaxLength(30, ErrorMessage = "שם לא יכול להכיל יותר מ-50 תווים")]
        public string? FirstName { get; set; }


        [Required(ErrorMessage = "שדה חובה")]
        [MaxLength(30, ErrorMessage = "שם לא יכול להכיל יותר מ-50 תווים")]
        public string? LastName { get; set; }


        [Required(ErrorMessage = "שדה חובה")]
        [MaxLength(255)]
        public string? Password { get; set; }


        [Required(ErrorMessage = "שדה חובה")]
        [EmailAddress(ErrorMessage = "כתובת מייל לא תקינה")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "שדה חובה")]
        [Phone(ErrorMessage = "מספר טלפון לא תקין")]
        public string? Phone { get; set; }
        public Role Role { get; set; }

        public virtual ICollection<TicketModel> Tickets { get; set; } = new List<TicketModel>();

      


    }
}
