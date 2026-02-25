using AutoMapper;
using server.Models;
using server.Models.DTO;
using System.Linq;

namespace server
{
    public class SaleProfile : Profile
    {
        public SaleProfile()
        {
            // =========================
            // 1. משתמשים (Users)
            // =========================
            CreateMap<UserDTO, UserModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());

            CreateMap<UserModel, UserDTO>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<UserModel, AuthUserDTO>();

            // =========================
            // 2. תורמים (Donors)
            // =========================
            CreateMap<DonorDTO, DonorModel>()
                 .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City)) // התאמה למה ששלחת קודם
                 .ForMember(dest => dest.Donations, opt => opt.Ignore());

            CreateMap<DonorModel, DonorDTO>()
                 .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

            // =========================
            // 3. קטגוריות (Categories)
            // =========================
            CreateMap<CategoryDTO, CategoryModel>()
                .ForMember(dest => dest.Gifts, opt => opt.Ignore());

            CreateMap<CategoryModel, CategoryDTO>();

            // =========================
            // 4. מתנות (Gifts) - מיפוי מפוצל ובטוח
            // =========================

            // מהשרת ללקוח (GET) - שליפה עם שמות מפורטים
            CreateMap<GiftModel, GiftDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
                .ForMember(dest => dest.DonorName, opt => opt.MapFrom(src => src.Donor != null ? src.Donor.FirstName + " " + src.Donor.LastName : ""))
                .ForMember(dest => dest.WinnerName, opt => opt.MapFrom(src => src.Winner != null ? src.Winner.FirstName : null))
                .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.Tickets != null ? src.Tickets.Where(t => t.IsPurchased).Sum(t => t.Quantity) : 0));

            // מהלקוח לשרת (POST/PUT) - מניעת יצירת אובייקטים מקושרים בטעות
            CreateMap<GiftDTO, GiftModel>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Donor, opt => opt.Ignore())
                .ForMember(dest => dest.Winner, opt => opt.Ignore())
                .ForMember(dest => dest.WinnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Tickets, opt => opt.Ignore());

            CreateMap<GiftModel, DonorGiftDTO>();

            // =========================
            // 5. סל קניות וכרטיסים (Cart & Tickets)
            // =========================

            // הוספה לסל (AddToCartDTO -> TicketModel)
            CreateMap<AddToCartDTO, TicketModel>()
             .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => DateTime.Now)) // נותן תאריך נוכחי בזמן ההוספה
             .ForMember(dest => dest.IsPurchased, opt => opt.MapFrom(src => false))
             .ForMember(dest => dest.Id, opt => opt.Ignore())
             .ForMember(dest => dest.BuyerId, opt => opt.Ignore());


            // שליפת הסל (TicketModel -> CartItemDTO)
            CreateMap<TicketModel, CartItemDTO>()
               .ForMember(dest => dest.GiftName, opt => opt.MapFrom(src => src.Gift != null ? src.Gift.Name : "מתנה לא ידועה"))
               .ForMember(dest => dest.GiftImage, opt => opt.MapFrom(src => src.Gift != null ? src.Gift.Image : ""))
               .ForMember(dest => dest.TicketPrice, opt => opt.MapFrom(src => src.Gift != null ? src.Gift.TicketPrice : 0))
               .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => (src.Gift != null) ? (src.Quantity * src.Gift.TicketPrice) : 0));

            // ניהול רכישות (TicketModel -> PurchaseDTO)
            CreateMap<TicketModel, PurchaseDTO>()
                .ForMember(dest => dest.GiftName, opt => opt.MapFrom(src => src.Gift != null ? src.Gift.Name : ""))
                .ForMember(dest => dest.GiftImage, opt => opt.MapFrom(src => src.Gift != null ? src.Gift.Image : ""))
                .ForMember(dest => dest.BuyerName, opt => opt.MapFrom(src => src.Buyer != null ? src.Buyer.FirstName + " " + src.Buyer.LastName : ""))
                .ForMember(dest => dest.BuyerEmail, opt => opt.MapFrom(src => src.Buyer != null ? src.Buyer.Email : ""))
                .ForMember(dest => dest.BuyerPhone, opt => opt.MapFrom(src => src.Buyer != null ? src.Buyer.Phone : ""))
                .ForMember(dest => dest.PricePerTicket, opt => opt.MapFrom(src => src.Gift != null ? src.Gift.TicketPrice : 0))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => (src.Gift != null) ? (src.Quantity * src.Gift.TicketPrice) : 0));
        }
    }
}