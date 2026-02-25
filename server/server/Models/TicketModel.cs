using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class TicketModel
    {
        public int Id { get; set; }
        [Required]
        public int GiftId { get; set; }
        public GiftModel? Gift { get; set; }

        [Required]
        public int BuyerId { get; set; }
        public UserModel? Buyer { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
        public bool IsPurchased { get; set; } = false;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? PurchaseDate { get; set; }
    }
}
