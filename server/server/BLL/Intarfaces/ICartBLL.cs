using server.Models.DTO;
namespace server.BLL.Intarfaces

{
    public interface ICartBLL
    {
        Task AddToCart(AddToCartDTO addToCartDto, int userId);
        Task<List<CartItemDTO>> GetCart(int userId);
        Task UpdateQuantity(int itemId, int userId, int quantity);
        Task RemoveFromCart(int itemId, int userId);

        // רכישה
        Task Checkout(int userId);
    }
}
