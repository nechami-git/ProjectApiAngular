using Microsoft.EntityFrameworkCore;
using server.DAL.Interfaces;
using server.Models;
using server.Models.DTO;
using System.Net.Sockets;

namespace server.DAL
{
    public class TicketDAL : ITicketDAL
    {
        private readonly ChineseSaleContext saleContext;
        public TicketDAL(ChineseSaleContext context)
        {
            saleContext = context;
        }

        // ניהול סל קניות
        public async Task<TicketModel?> GetTicketInCart(int userId, int giftId)
        {
            return await saleContext.Tickets
                .FirstOrDefaultAsync(t => t.BuyerId == userId && t.GiftId == giftId && !t.IsPurchased);
        }

        public async Task Add(TicketModel ticket)
        {
            await saleContext.Tickets.AddAsync(ticket);
            await saleContext.SaveChangesAsync();
        }

        public async Task Update(TicketModel ticket)
        {

            saleContext.Tickets.Update(ticket);
            await saleContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var ticket = await saleContext.Tickets.FindAsync(id);
            if (ticket != null)
            {
                saleContext.Tickets.Remove(ticket);
                await saleContext.SaveChangesAsync();
            }
        }

        public async Task<TicketModel?> GetById(int id)
        {
            return await saleContext.Tickets.FindAsync(id);
        }

        public async Task<List<TicketModel>> GetCartByUserId(int userId)
        {
            return await saleContext.Tickets
                 .Include(t => t.Gift) // מביא את פרטי המתנה להצגה בסל
                 .Where(t => t.BuyerId == userId && !t.IsPurchased)
                 .ToListAsync();
        }

        // הגרלה
        public async Task<List<TicketModel>> GetLottoTickets(int giftId)
        {
            return await saleContext.Tickets
                .Include(t => t.Buyer)
                .Where(t => t.GiftId == giftId && t.IsPurchased)
                .ToListAsync();
        }

        // רכישה
        public async Task PayForCart(int userId)
        {
            var cartTickets = await saleContext.Tickets
                .Where(t => t.BuyerId == userId && !t.IsPurchased)
                .ToListAsync();

            if (cartTickets.Any())
            {
                foreach (var ticket in cartTickets)
                {
                    ticket.IsPurchased = true;
                    ticket.PurchaseDate = DateTime.Now;
                }
                await saleContext.SaveChangesAsync();
            }
        }

        // --- ניהול רכישות ---
        public async Task<List<TicketModel>> GetAllPurchases(string? sortBy = null)
        {
            var query = saleContext.Tickets
                .Include(t => t.Gift)
                    .ThenInclude(g => g.Category)
                .Include(t => t.Gift)
                    .ThenInclude(g => g.Donor)
                .Include(t => t.Buyer)
                .Where(t => t.IsPurchased)
                .AsQueryable();


            // מיון ב-SQL - כמו ב-GiftDAL!
            query = sortBy?.ToLower() switch
            {
                "expensive" => query.OrderByDescending(t => t.Gift.TicketPrice),
                "mostpurchased" => query.OrderByDescending(t => t.Quantity),
                _ => query.OrderByDescending(t => t.PurchaseDate) // ברירת מחדל: לפי תאריך
            };

            return await query.ToListAsync();
        }

        public async Task<List<TicketModel>> GetPurchasesByGiftId(int giftId)
        {
            return await saleContext.Tickets
                .Include(t => t.Gift)
                .Include(t => t.Buyer)
                .Where(t => t.GiftId == giftId && t.IsPurchased)
                .OrderByDescending(t => t.PurchaseDate)
                .ToListAsync();
        }

        public async Task<List<TicketModel>> GetPurchasesByUserId(int userId)
        {
            return await saleContext.Tickets
                .Include(t => t.Gift)
                    .ThenInclude(g => g.Category)
                .Where(t => t.BuyerId == userId && t.IsPurchased)
                .OrderByDescending(t => t.PurchaseDate)
                .ToListAsync();
        }

        // קיבוץ רוכשים לפי מתנה - עם חישובים ב-SQL
        public async Task<List<PurchaserDetailDTO>> GetPurchasersGroupedByGift(int giftId)
        {
            // ביצוע קיבוץ ב-SQL - לא בזיכרון!
            return await saleContext.Tickets
                .Include(t => t.Buyer)
                .Include(t => t.Gift)
                .Where(t => t.GiftId == giftId && t.IsPurchased)
                .GroupBy(t => new
                {
                    t.BuyerId,
                    t.Buyer!.FirstName,
                    t.Buyer.LastName,
                    t.Buyer.Email,
                    t.Buyer.Phone,
                    TicketPrice = t.Gift!.TicketPrice
                })
                .Select(g => new PurchaserDetailDTO
                {
                    UserId = g.Key.BuyerId,
                    Name = g.Key.FirstName + " " + g.Key.LastName,
                    Email = g.Key.Email,
                    Phone = g.Key.Phone,
                    TotalTickets = g.Sum(t => t.Quantity),
                    TotalSpent = g.Sum(t => t.Quantity * g.Key.TicketPrice),
                    FirstPurchaseDate = g.Min(t => t.PurchaseDate)
                })
                .OrderByDescending(p => p.TotalTickets)
                .ToListAsync(); // GroupBy + Select + OrderBy כולם מתבצעים ב-SQL!
        }
    }
}
