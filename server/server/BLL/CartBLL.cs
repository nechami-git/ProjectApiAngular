using AutoMapper;
using Microsoft.EntityFrameworkCore;
using server.BLL.Intarfaces;
using server.DAL.Interfaces;
using server.Exceptions;
using server.Models;
using server.Models.DTO;

namespace server.BLL
{
    public class CartBLL : ICartBLL
    {
        private readonly ITicketDAL _ticketDAL;
        private readonly IGiftDAL _giftDAL;
        private readonly IMapper _mapper;

        public CartBLL(ITicketDAL ticketDAL, IGiftDAL giftDAL, IMapper mapper)
        {
            _ticketDAL = ticketDAL;
            _giftDAL = giftDAL;
            _mapper = mapper;
        }

        public async Task AddToCart(AddToCartDTO dto, int userId)
        {
            var gift = await _giftDAL.GetById(dto.GiftId);
            if (gift == null)
                throw new NotFoundException("המתנה לא קיימת");

            if (gift.WinnerId != null)
            {
                throw new BusinessException("ההגרלה למתנה זו כבר הסתיימה, לא ניתן לרכוש כרטיסים.");
            }

            var existingTicket = await _ticketDAL.GetTicketInCart(userId, dto.GiftId);
            if (existingTicket != null)
            {
                existingTicket.Quantity += dto.Quantity;
                existingTicket.PurchaseDate = DateTime.Today;
                await _ticketDAL.Update(existingTicket);
            }
            else
            {
                // יצירת פריט חדש בסל
                var newTicket = _mapper.Map<TicketModel>(dto);
                newTicket.BuyerId = userId;
                newTicket.IsPurchased = false;
                await _ticketDAL.Add(newTicket);
            }
        }

        public async Task Checkout(int userId)
        {
            var cart = await _ticketDAL.GetCartByUserId(userId);
            if (cart == null || !cart.Any())
                throw new BusinessException("סל הקניות ריק");

            // בדיקה שאף אחת מהמתנות לא הוגרלה כבר
            foreach (var ticket in cart)
            {
                var gift = await _giftDAL.GetById(ticket.GiftId);
                if (gift?.WinnerId != null)
                {
                    throw new BusinessException($"המתנה '{gift.Name}' כבר הוגרלה. אנא הסר אותה מהסל לפני השלמת הרכישה.");
                }
            }

            await _ticketDAL.PayForCart(userId);
        }

        public async Task<List<CartItemDTO>> GetCart(int userId)
        {
            var tickets = await _ticketDAL.GetCartByUserId(userId);
            // המיפוי כאן קריטי - הוא הופך את הנתונים ל-DTO כולל חישוב מחירים
            return _mapper.Map<List<CartItemDTO>>(tickets);
        }

        public async Task RemoveFromCart(int itemId, int userId)
        {
            var ticket = await _ticketDAL.GetById(itemId);
            if (ticket == null) throw new NotFoundException("הפריט לא נמצא בסל");

            if (ticket.BuyerId != userId) throw new UnauthorizedException("גישה נדחתה");
            if (ticket.IsPurchased) throw new BusinessException("לא ניתן למחוק פריט שכבר נרכש");

            await _ticketDAL.Delete(itemId);
        }

        public async Task UpdateQuantity(int giftId, int userId, int quantity) 
        {
            if (quantity <= 0) throw new BusinessException("כמות חייבת להיות חיובית");

            var ticket = await _ticketDAL.GetTicketInCart(userId, giftId);

            if (ticket == null) throw new NotFoundException("הפריט לא נמצא בסל");

            if (ticket.IsPurchased) throw new BusinessException("לא ניתן לעדכן פריט שכבר נרכש");

            ticket.Quantity = quantity;
            await _ticketDAL.Update(ticket);
        }
    }
}
