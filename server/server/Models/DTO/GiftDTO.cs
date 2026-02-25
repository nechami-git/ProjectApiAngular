using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace server.Models.DTO
{
    public class GiftDTO
    {
        //פרטים בסיסיים של מתנה
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int Price { get; set; }
        public decimal TicketPrice { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int DonorId { get; set; }
        public string? DonorName { get; set; }

        public int ParticipantsCount { get; set; }

        // WinnerName: לטובת דוח הזוכים לאחר ההגרלה
        public string? WinnerName { get; set; }
    }
}
