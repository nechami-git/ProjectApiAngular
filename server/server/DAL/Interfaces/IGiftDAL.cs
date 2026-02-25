using server.Models;
using server.Models.DTO;

namespace server.DAL.Interfaces
{
    public interface IGiftDAL
    {   
        Task<List<GiftModel>> GetAdmin(string? name = null, string? donorName = null, int? purchaseCount = null, string? sortBy = null);
        Task<List<GiftModel>> GetGifts(string? name = null, string? CategoryName = null, int? TicketPrice = null);
        Task<GiftModel?> GetById(int id);
        Task<int> Post(GiftModel gift);
        Task<GiftModel?> Put(GiftModel gift);
        Task Delete(int id);
        Task<bool> HasPurchases(int id);
        Task UpdateWinner(int giftId, int userId);
    }
}
