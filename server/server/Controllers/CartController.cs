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
    public class CartController : ControllerBase
    {
        private readonly ICartBLL _cartBll;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartBLL cartBll, ILogger<CartController> logger)
        {
            _cartBll = cartBll;
            _logger = logger;
        }

        // --- פונקציית עזר פרטית לחילוץ ה-ID מהטוקן ---
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

        // POST api/cart/add

        [HttpPost("add")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out int userId))
            {
                _logger.LogWarning("User ID not found in token when adding to cart");
                return Unauthorized(new { message = "לא נמצא מזהה משתמש בטוקן" });
            }

            _logger.LogInformation($"Adding item to cart for User: {userId}, Gift: {dto.GiftId}");

            await _cartBll.AddToCart(dto, userId);
            return Ok(new { message = "הפריט התווסף לסל בהצלחה" });
        }


        // GET api/cart
        [HttpGet]
        public async Task<ActionResult<List<CartItemDTO>>> GetCart()
        {
            if (!TryGetUserId(out int userId))
            {
                _logger.LogWarning("User ID not found in token when fetching cart");
                return Unauthorized(new { message = "לא נמצא מזהה משתמש בטוקן" });
            }

            _logger.LogInformation($"Fetching cart for user id: {userId}");

            var cart = await _cartBll.GetCart(userId);

            if (cart == null || !cart.Any())
            {
                _logger.LogInformation($"Cart is empty for user {userId}");
                return Ok(new List<CartItemDTO>());
            }

            _logger.LogDebug($"Retrieved {cart.Count} items for user {userId}");
            return Ok(cart);
        }

        // PUT api/cart/update
        [HttpPut("update")]
        public async Task<ActionResult> UpdateQuantity([FromBody] UpdateCartItemDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out int userId))
            {
                _logger.LogWarning("User ID not found in token when updating quantity");
                return Unauthorized(new { message = "לא נמצא מזהה משתמש בטוקן" });
            }

            _logger.LogInformation($"Updating cart item quantity. ItemId: {dto.ItemId}, User: {userId}");
            _logger.LogDebug($"New Quantity: {dto.Quantity}");

            await _cartBll.UpdateQuantity(dto.ItemId, userId, dto.Quantity);
            return Ok(new { message = "הכמות עודכנה בהצלחה" });
        }

        // DELETE api/cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            if (!TryGetUserId(out int userId))
            {
                _logger.LogWarning("User ID not found in token when removing from cart");
                return Unauthorized(new { message = "לא נמצא מזהה משתמש בטוקן" });
            }

            _logger.LogInformation($"Removing item {id} from cart for user: {userId}");
            await _cartBll.RemoveFromCart(id, userId);
            return Ok(new { message = "הפריט הוסר מהסל" });
        }

        // POST api/cart/checkout
        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout()
        {
            if (!TryGetUserId(out int userId))
            {
                _logger.LogWarning("User ID not found in token when checking out");
                return Unauthorized(new { message = "לא נמצא מזהה משתמש בטוקן" });
            }

            _logger.LogInformation($"Starting checkout process for user: {userId}");

            await _cartBll.Checkout(userId);

            _logger.LogInformation($"Checkout completed successfully for user: {userId}");
            return Ok(new { message = "הרכישה בוצעה בהצלחה!" });
        }
    }
}
