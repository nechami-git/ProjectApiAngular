using server.Models;
using server.Models.DTO;

namespace server.BLL.Intarfaces
{
    public interface IGiftBLL
    {
      
        Task<List<GiftDTO>> GetAdmin(string? name = null, string? donorName = null, int? purchaseCount = null, string? sortBy = null);
        Task<List<GiftDTO>> GetGifts(string? name = null, string? CategoryName = null, int? TicketPrice = null);
        Task<GiftDTO?> GetById(int id);
        Task<int> Post(GiftDTO giftDto);
        Task<GiftDTO?> Put(int id, GiftDTO giftDto);
        Task Delete(int id);
        Task <GiftDTO?> PerformRaffle(int giftId);
        Task<byte[]> GenerateReport();
    }
}
