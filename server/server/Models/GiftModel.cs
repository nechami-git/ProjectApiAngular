using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;

namespace server.Models
{
    public class GiftModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        [MaxLength(50, ErrorMessage = "שם לא יכול להיות יותר מ-50 תווים")]
        [MinLength(2, ErrorMessage = "שם המתנה חייב להכיל לפחות 2 תווים")]
        public string? Name { get; set; }



        [Required(ErrorMessage = "שדה חובה")]
        [StringLength(500, ErrorMessage = "התיאור ארוך מדי")]
        public string? Description { get; set; }


        [Required(ErrorMessage = "חובה להוסיף תמונה למתנה")]
        public string? Image { get; set; }


        [Required(ErrorMessage = "חובה לשייך תורם למתנה")]
        public int DonorId { get; set; }
        [ForeignKey("DonorId")]
        public DonorModel? Donor { get; set; }


        [Required(ErrorMessage = "חובה להזין את שווי המתנה")]
        [Range(1, 1000000, ErrorMessage = "שווי המתנה חייב להיות מספר חיובי")]
        public int Price { get; set; } ///??????????


        [Required(ErrorMessage = "חובה להזין מחיר לכרטיס הגרלה")]
        [Range(1, 100, ErrorMessage = "₪ מחיר כרטיס חייב להיות בין 1 ל-100 ")]
        public decimal TicketPrice { get; set; }



        [Required(ErrorMessage = "חובה לבחור קטגוריה")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public CategoryModel? Category { get; set; }


        public virtual ICollection<TicketModel> Tickets { get; set; } = new List<TicketModel>();

        public int? WinnerId { get; set; }
        [ForeignKey("WinnerId")]
        public UserModel? Winner { get; set; }


        [NotMapped]
        public int TicketsCount => Tickets.Count;

    }
}
