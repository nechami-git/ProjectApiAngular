using System.ComponentModel.DataAnnotations; // חובה להוסיף את זה בשביל Required

namespace server.Models.DTO
{
    public class DonorDTO
    {
        public int Id { get; set; }
        public string? IdentityNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }


        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "פורמט אימייל לא תקין")]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public IEnumerable<DonorGiftDTO> Donations { get; set; } = new List<DonorGiftDTO>();
    }
}