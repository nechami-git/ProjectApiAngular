using server.Models;
using server.Models.DTO;

namespace server.DAL.Interfaces
{
    public interface ITicketDAL
    {
        // --- צד לקוח (סל קניות) ---
        Task Add(TicketModel ticket); 
        Task Update(TicketModel ticket); 
        Task Delete(int id); 
        Task<TicketModel?> GetTicketInCart(int userId, int giftId); 
        Task<List<TicketModel>> GetCartByUserId(int userId); 
        Task<TicketModel?> GetById(int id);


        // --- צד הנהלה (הגרלות ודוחות) ---
        Task<List<TicketModel>> GetLottoTickets(int giftId);

        // --- תהליך רכישה ---
        Task PayForCart(int userId); 

        // --- ניהול רכישות (מסך הנהלה) ---
        Task<List<TicketModel>> GetAllPurchases(
            string? sortBy = null
        ); 
        Task<List<TicketModel>> GetPurchasesByGiftId(int giftId); // רכישות למתנה מסוימת
        Task<List<TicketModel>> GetPurchasesByUserId(int userId); // רכישות של משתמש מסוים

        Task<List<PurchaserDetailDTO>> GetPurchasersGroupedByGift(int giftId);
    }
}
