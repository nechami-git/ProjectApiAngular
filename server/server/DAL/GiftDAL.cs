using Microsoft.EntityFrameworkCore;
using server.DAL.Interfaces;
using server.Models;
using server.Models.DTO;


namespace server.DAL
{
    public class GiftDAL : IGiftDAL
    {

        private readonly ChineseSaleContext _dbContext;

        public GiftDAL(ChineseSaleContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<GiftModel>> GetAdmin(string? name = null, string? donorName = null, int? purchaseCount = null, string? sortBy = null)
        {
            var query = _dbContext.Gifts
                 .Include(g=>g.Category)
                 .Include(g=>g.Donor)
                 .Include(g=>g.Tickets)
                 .Include(g=>g.Winner)
                 .AsQueryable();;

            switch (sortBy)
            {
                case "price": query = query.OrderBy(g => g.TicketPrice); break;
                case "mostPurchased": query = query.OrderByDescending(g => g.Tickets.Count(t => t.IsPurchased)); break;
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(g => g.Name!.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(donorName))
            {
                query = query.Where(g => g.Donor!.FirstName!.Contains(donorName) || g.Donor!.LastName!.Contains(donorName));
            }
            if (purchaseCount.HasValue)
            {
                query = query.Where(g => g.Tickets.Where(t => t.IsPurchased)
                .Sum(t => t.Quantity) >= purchaseCount.Value);
            }
            return await query.AsNoTracking().ToListAsync();
        }
        public async Task<List<GiftModel>> GetGifts(string? name = null, string? CategoryName = null, int? TicketPrice = null)
        {
            var query = _dbContext.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include(g => g.Tickets)
                .Include(g => g.Winner)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(g => g.Name!.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(CategoryName))
            {
                query = query.Where(g => g.Category!.Name!.Contains(CategoryName));
            }
            if (TicketPrice.HasValue)
            {
                query = query.Where(g => g.TicketPrice == TicketPrice);
            }

            return await query.AsNoTracking().ToListAsync();
        }
        public async Task<GiftModel?> GetById(int id)
        {
            return await _dbContext.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include(g => g.Tickets).ThenInclude(t => t.Buyer)
                .Include(g => g.Winner)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
        public async Task<int> Post(GiftModel gift)
        {
            _dbContext.Gifts.Add(gift);
            await _dbContext.SaveChangesAsync();
            return gift.Id;
        }
       
        public async Task<GiftModel?> Put(GiftModel gift)
        {
            var existingGift = await _dbContext.Gifts.FindAsync(gift.Id);
            if (existingGift == null)
            {
                return null;
            }
            _dbContext.Entry(existingGift).CurrentValues.SetValues(gift);
            await _dbContext.SaveChangesAsync();
            return existingGift;
        }
        public async Task Delete(int id)
        {
            var gift = await _dbContext.Gifts.FindAsync(id);
            if (gift != null)
            {
                _dbContext.Gifts.Remove(gift);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<bool> HasPurchases(int id)
        {
            return await _dbContext.Tickets.AnyAsync(t => t.GiftId == id && t.IsPurchased);
        }
        public async Task UpdateWinner(int giftId, int userId)
        {
            var gift = await _dbContext.Gifts.FindAsync(giftId);
            if (gift != null)
            {
                gift.WinnerId = userId;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
