using AutoMapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using server.BLL.Intarfaces;
using server.DAL;
using server.DAL.Interfaces;
using server.Exceptions;
using server.Models;
using server.Models.DTO;
using System.Drawing;
using System.Globalization;

namespace server.BLL
{
    public class GiftBLL : IGiftBLL
    {
        private readonly IGiftDAL _giftDAL;
        private readonly IMapper _mapper;
        private readonly ITicketDAL _ticketDAL;
        private readonly IEmailBLL _emailBLL;
        private readonly ILogger<GiftBLL> _logger;
        public GiftBLL(IGiftDAL giftDAL, IMapper mapper, IEmailBLL emailBLL, ITicketDAL ticketDAL, ILogger<GiftBLL> logger)
        {
            _giftDAL = giftDAL;
            _mapper = mapper;
            _ticketDAL = ticketDAL;
            _emailBLL = emailBLL;
            _logger = logger;
        }

        public async Task<List<GiftDTO>> GetAdmin(string? name = null, string? donorName = null, int? purchaseCount = null, string? sortBy = null)
        {
            var gifts = await _giftDAL.GetAdmin(name, donorName, purchaseCount, sortBy);

            return _mapper.Map<List<GiftDTO>>(gifts);
        }
        public async Task<List<GiftDTO>> GetGifts(string? name = null, string? CategoryName = null, int? TicketPrice = null)
        {
            var gifts = await _giftDAL.GetGifts(name,CategoryName, TicketPrice);
            return _mapper.Map<List<GiftDTO>>(gifts);
        }

        public async Task<GiftDTO?> GetById(int id)
        {
            var gift = await _giftDAL.GetById(id);
            if (gift == null)
                throw new NotFoundException($"מתנה עם מזהה {id} לא נמצאה.");

            return _mapper.Map<GiftDTO>(gift);
        }

        public async Task<int> Post(GiftDTO giftDto)
        {
            if (giftDto == null)
            {
                throw new ArgumentNullException(nameof(giftDto), "Gift data cannot be null.");
            }
            if (giftDto.TicketPrice >= giftDto.Price)
            {
                throw new ArgumentException("מחיר כרטיס הגרלה חייב להיות נמוך משווי המתנה");
            }
            var giftModel = _mapper.Map<GiftModel>(giftDto);
            return await _giftDAL.Post(giftModel);
        }

        public async Task<GiftDTO?> Put(int id, GiftDTO giftDto)
        {
            var existingGift = await _giftDAL.GetById(id);
            if (existingGift == null)
            {
                throw new NotFoundException($"לא ניתן לעדכן, מתנה {id} לא נמצאה.");
            }

            var giftToUpdate = _mapper.Map<GiftModel>(giftDto);
            giftToUpdate.Id = id; 

            var updatedGift = await _giftDAL.Put(giftToUpdate);
            return _mapper.Map<GiftDTO>(updatedGift);
        }

        public async Task Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid ID");
            }
            var existingGift = await _giftDAL.GetById(id);
            if (existingGift == null)
            {
                throw new KeyNotFoundException($"Cannot delete. Gift with ID {id} not found.");
            }
            bool hasPurchases = await _giftDAL.HasPurchases(id);
            if (hasPurchases)
            {
                throw new InvalidOperationException("לא ניתן למחוק מתנה לאחר שנרכשו עבורה כרטיסים.");
            }
            await _giftDAL.Delete(id);
        }

        public async Task<GiftDTO?> PerformRaffle(int giftId)
        {
            var gift = await _giftDAL.GetById(giftId);
            if (gift == null)
                throw new NotFoundException("המתנה לא נמצאה במערכת");

            if (gift.WinnerId != null)
                throw new BusinessException("ההגרלה למתנה זו כבר התקיימה!");

            var tickets = await _ticketDAL.GetLottoTickets(giftId);

            if (tickets == null || !tickets.Any())
                throw new BusinessException("לא נרכשו כרטיסים למתנה זו, לא ניתן לבצע הגרלה.");


            int totalTickets = tickets.Sum(t => t.Quantity);

            Random rand = new Random();
            int winningTicketNumber = rand.Next(1, totalTickets + 1);

            int currentCount = 0;
            UserModel? winner = null;

            foreach (var ticket in tickets)
            {
                currentCount += ticket.Quantity;
                if (currentCount >= winningTicketNumber)
                {
                    winner = ticket.Buyer;
                    break;
                }
            }

            if (winner == null) throw new InternalServerException("שגיאה בחישוב הזוכה");

            await _giftDAL.UpdateWinner(giftId, winner.Id);

            if (gift.Winner != null && !string.IsNullOrEmpty(gift.Winner.Email))
            {
                string subject = "מזל טוב! זכית במכירה הסינית! 🎉";
                string body = $@"
                    <div style='direction: rtl; text-align: right; font-family: Arial;'>
                        <h1>שלום {winner.FirstName},</h1>
                        <p style='font-size: 18px;'>שמחים להודיע לך שזכית בהגרלה על המתנה:</p>
                        <h2 style='color: blue;'>🎁 {gift.Name} 🎁</h2>
                        <p>המתנה ממתינה לך במשרדי הארגון.</p>
                        <br>
                        <p>בברכה,<br>צוות המכירה הסינית</p>
                    </div>";
                try
                {
                    await _emailBLL.SendEmail(gift.Winner.Email, subject, body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send email to winner: {gift.Winner.Email} for gift: {gift.Name}");
                }
            }

            var updatedGift = await _giftDAL.GetById(giftId);
            return _mapper.Map<GiftDTO>(updatedGift);
        }
        // יצירת דוח זוכים בפורמט Excel
        public async Task<byte[]> GenerateReport()
        {
            // 1. שליפת הנתונים
            var gifts = await _giftDAL.GetGifts();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("דוח זוכים");

                // RTL support
                worksheet.View.RightToLeft = true;

                // הגדרת כותרת ראשית
                worksheet.Cells["A1:F1"].Merge = true;
                worksheet.Cells["A1"].Value = $"דוח מסכם - מכירה סינית - {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                // כותרות עמודות
                worksheet.Cells["A3"].Value = "שם המתנה";
                worksheet.Cells["B3"].Value = "זוכה";
                worksheet.Cells["C3"].Value = "טלפון";
                worksheet.Cells["D3"].Value = "כמות כרטיסים";
                worksheet.Cells["E3"].Value = "הכנסות (₪)";
                worksheet.Cells["F3"].Value = "סטטוס";

                // עיצוב כותרות
                using (var range = worksheet.Cells["A3:F3"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // מילוי נתונים
                int row = 4;
                decimal totalIncome = 0;

                foreach (var gift in gifts)
                {
                    var ticketsSold = gift.Tickets.Where(t => t.IsPurchased).Sum(t => t.Quantity);
                    var income = ticketsSold * gift.TicketPrice;
                    totalIncome += income;

                    worksheet.Cells[row, 1].Value = gift.Name;

                    if (gift.Winner != null)
                    {
                        worksheet.Cells[row, 2].Value = $"{gift.Winner.FirstName} {gift.Winner.LastName}";
                        worksheet.Cells[row, 3].Value = gift.Winner.Phone;
                        worksheet.Cells[row, 6].Value = "בוצעה הגרלה ✓";
                        worksheet.Cells[row, 6].Style.Font.Color.SetColor(Color.Green);
                    }
                    else
                    {
                        worksheet.Cells[row, 2].Value = "טרם בוצעה הגרלה";
                        worksheet.Cells[row, 3].Value = "-";
                        worksheet.Cells[row, 6].Value = "ממתין להגרלה";
                        worksheet.Cells[row, 6].Style.Font.Color.SetColor(Color.Orange);
                    }

                    worksheet.Cells[row, 4].Value = ticketsSold;
                    worksheet.Cells[row, 5].Value = income;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0 ₪";

                    // גבולות לשורה
                    worksheet.Cells[row, 1, row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    row++;
                }

                // סיכום סופי
                row++; // שורה ריקה
                worksheet.Cells[row, 1].Value = "סה\"כ הכנסות כלליות:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = totalIncome;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0 ₪";
                worksheet.Cells[row, 2].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 2].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                // התאמת רוחב עמודות
                worksheet.Column(1).Width = 25; // שם המתנה
                worksheet.Column(2).Width = 20; // זוכה
                worksheet.Column(3).Width = 15; // טלפון
                worksheet.Column(4).Width = 15; // כמות כרטיסים
                worksheet.Column(5).Width = 15; // הכנסות
                worksheet.Column(6).Width = 20; // סטטוס

                // החזרת הקובץ כ-byte array
                return package.GetAsByteArray();
            }
        }
    }
}
