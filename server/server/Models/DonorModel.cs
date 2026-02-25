using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class DonorModel
    {  
        public int Id { get; set; }

        [Required]
        [StringLength(9)]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "תעודת זהות חייבת להכיל 9 ספרות")]
        public string? IdentityNumber { get; set; }
     

        [Required(ErrorMessage = "שדה חובה")]
        [MaxLength(50, ErrorMessage = "שם פרטי לא יכול להכיל יותר מ-50 תווים")]
        public string? FirstName { get; set; }


        [Required(ErrorMessage = "שדה חובה")]
        [MaxLength(50, ErrorMessage = "שם משפחה לא יכול להכיל יותר מ-50 תווים")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        public string? City { get; set; }


        [Required(ErrorMessage = "שדה חובה")]
        [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "שדה חובה")]
        [Phone(ErrorMessage = "מספר טלפון לא תקין")]
        public string? Phone { get; set; }
        public virtual ICollection<GiftModel> Donations { get; set; } = new List<GiftModel>();

    }
}


