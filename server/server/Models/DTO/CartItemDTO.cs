using System.ComponentModel.DataAnnotations;

namespace server.Models.DTO
{
    // DTO להוספת פריט לסל (Input)
    public class AddToCartDTO
    {
        [Required(ErrorMessage = "חובה לציין מזהה מתנה")]
        public int GiftId { get; set; }

        [Range(1, 100, ErrorMessage = "כמות חייבת להיות בין 1 ל-100")]
        public int Quantity { get; set; } = 1;
    }

    // DTO לעדכון כמות פריט בסל (Input)
    public class UpdateCartItemDTO
    {
        [Required]
        public int ItemId { get; set; } 

        [Required]
        [Range(1, 100, ErrorMessage = "כמות חייבת להיות מספר חיובי עד 100")]
        public int Quantity { get; set; }
    }

    // DTO לתצוגת פריט בסל (Output)
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int GiftId { get; set; }
        public string? GiftName { get; set; }
        public string? GiftImage { get; set; }
        public int TicketPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; } 
    }
}
