using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.BLL.Intarfaces;
using server.Models.DTO;
using System.Security.Claims;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseBLL _purchaseBLL;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(IPurchaseBLL purchaseBLL, ILogger<PurchaseController> logger)
        {
            _purchaseBLL = purchaseBLL;
            _logger = logger;
        }
        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == "id") ??
                            User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userClaim != null && int.TryParse(userClaim.Value, out userId))
            {
                return true;
            }
            return false;
        }

        // GET api/purchase
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<PurchaseDTO>>> GetAllPurchases([FromQuery] string? sortBy)
        {
            _logger.LogInformation("Manager fetching all purchases - Sort: {SortBy}",
                sortBy ?? "default");

            var purchases = await _purchaseBLL.GetAllPurchases(sortBy);

            _logger.LogInformation("Retrieved {Count} purchases", purchases.Count);
            return Ok(purchases);
        }

        // GET api/purchase/gift/{giftId}
        [HttpGet("gift/{giftId}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<GiftPurchasersDTO>> GetPurchasersByGift(int giftId)
        {
            _logger.LogInformation("Manager fetching purchasers for gift: {GiftId}", giftId);

            var purchasers = await _purchaseBLL.GetPurchasersByGift(giftId);

            _logger.LogInformation("Found {Count} purchasers for gift {GiftId}",
                purchasers.Purchasers.Count, giftId);
            return Ok(purchasers);
        }

        // GET api/purchase/my-purchases
        [HttpGet("my-purchases")]
        public async Task<ActionResult<List<PurchaseDTO>>> GetMyPurchases()
        {
            if (!TryGetUserId(out int userId))
            {
                _logger.LogWarning("User ID not found in token");
                return Unauthorized(new { message = "לא נמצא מזהה משתמש בטוקן" });
            }

            _logger.LogInformation("User {UserId} fetching their purchases", userId);

            var purchases = await _purchaseBLL.GetUserPurchases(userId);

            _logger.LogInformation("User {UserId} has {Count} purchases", userId, purchases.Count);
            return Ok(purchases);
        }

        // GET api/purchase/user/{userId}
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<List<PurchaseDTO>>> GetUserPurchases(int userId)
        {
            _logger.LogInformation("Manager fetching purchases for user: {UserId}", userId);

            var purchases = await _purchaseBLL.GetUserPurchases(userId);

            _logger.LogInformation("User {UserId} has {Count} purchases", userId, purchases.Count);
            return Ok(purchases);
        }
    }
}
