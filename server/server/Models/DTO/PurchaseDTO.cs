namespace server.Models.DTO
{
    // DTO להצגת רכישה בודדת (לניהול רכישות)
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public int GiftId { get; set; }
        public string? GiftName { get; set; }
        public string? GiftImage { get; set; }
        public int BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public string? BuyerEmail { get; set; }
        public string? BuyerPhone { get; set; }
        public int Quantity { get; set; }
        public int PricePerTicket { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
    }

    //  להצגת רוכשים של מתנה מסוימת
    public class GiftPurchasersDTO
    {
        public int GiftId { get; set; }
        public string? GiftName { get; set; }
        public List<PurchaserDetailDTO> Purchasers { get; set; } = new List<PurchaserDetailDTO>();
    }

    public class PurchaserDetailDTO
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int TotalTickets { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? FirstPurchaseDate { get; set; }
    }
}
