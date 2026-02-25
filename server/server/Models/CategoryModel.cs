using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "שם הקטגוריה הוא שדה חובה")]
        [MaxLength(30)]
        public string? Name { get; set; }

        // קשר הפוך - רשימת המתנות השייכות לקטגוריה זו
        public virtual ICollection<GiftModel> Gifts { get; set; } = new List<GiftModel>();
    }
}
