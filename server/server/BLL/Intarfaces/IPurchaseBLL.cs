using server.Models.DTO;

namespace server.BLL.Intarfaces
{
    public interface IPurchaseBLL
    {
        Task<List<PurchaseDTO>> GetAllPurchases(string? sortBy = null);
        Task<GiftPurchasersDTO> GetPurchasersByGift(int giftId);
        Task<List<PurchaseDTO>> GetUserPurchases(int userId);
    }
}
