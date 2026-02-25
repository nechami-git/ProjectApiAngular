using AutoMapper;
using server.BLL.Intarfaces;
using server.DAL;
using server.DAL.Interfaces;
using server.Exceptions;
using server.Models;
using server.Models.DTO;

namespace server.BLL
{
    public class PurchaseBLL : IPurchaseBLL
    {
        private readonly ITicketDAL _ticketDAL;
        private readonly IGiftDAL _giftDAL;
        private readonly IMapper _mapper;

        public PurchaseBLL(ITicketDAL ticketDAL, IMapper mapper,IGiftDAL giftDAL)
        {
            _ticketDAL = ticketDAL;
            _mapper = mapper;
            _giftDAL = giftDAL;
        }

        public async Task<List<PurchaseDTO>> GetAllPurchases(string? sortBy = null)
        {

            var purchases = await _ticketDAL.GetAllPurchases(sortBy);

            return _mapper.Map<List<PurchaseDTO>>(purchases);
        }

        public async Task<GiftPurchasersDTO> GetPurchasersByGift(int giftId)
        {
            var gift = await _giftDAL.GetById(giftId);
            if (gift == null)
                throw new NotFoundException("המתנה לא נמצאה");

            // הקיבוץ מתבצע ב-SQL דרך ה-DAL - כמו בשאר הפונקציות!
            var purchaserGroups = await _ticketDAL.GetPurchasersGroupedByGift(giftId);

            return new GiftPurchasersDTO
            {
                GiftId = giftId,
                GiftName = gift.Name,
                Purchasers = purchaserGroups
            };
        }

        public async Task<List<PurchaseDTO>> GetUserPurchases(int userId)
        {
            var purchases = await _ticketDAL.GetPurchasesByUserId(userId);
            return _mapper.Map<List<PurchaseDTO>>(purchases);
        }



    }
}
