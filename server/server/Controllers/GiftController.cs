using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using server.BLL;
using server.BLL.Intarfaces;
using server.Models;
using server.Models.DTO;
using System.Text;


namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly ILogger<GiftController> _logger;
        private readonly IGiftBLL _giftBLL;
        private readonly IEmailBLL _emailBLL;

        public GiftController(ILogger<GiftController> logger, IGiftBLL giftBLL, IEmailBLL emailBLL)
        {
            _logger = logger;
            _giftBLL = giftBLL;
            _emailBLL = emailBLL;
        }

        // GET: api/<GiftController>
        [HttpGet("Manager")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<GiftDTO>>> GetAdmin([FromQuery] string? name, [FromQuery] string? donorName, [FromQuery] int? purchaseCount)
        {
            _logger.LogInformation("Fetching gifts with filters");
            var gifts = await _giftBLL.GetAdmin(name, donorName, purchaseCount);
            return Ok(gifts);
        }
        // GET: api/<GiftController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiftDTO>>> Get([FromQuery] string? name, [FromQuery] string? CategoryName, [FromQuery] int? TicketPrice)
        {
            _logger.LogInformation("Fetching gifts with filters");
            var gifts = await _giftBLL.GetGifts(name, CategoryName, TicketPrice);
            return Ok(gifts);
        }


        // GET api/<GiftController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GiftDTO?>> Get(int id)
        {
            _logger.LogInformation("Fetching gift with ID: {Id}", id);
            var gift = await _giftBLL.GetById(id);
            return Ok(gift);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Post(GiftDTO giftDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Manager adding new gift: {GiftName}", giftDto.Name);
            var newId = await _giftBLL.Post(giftDto);
            return CreatedAtAction(nameof(Get), new { id = newId }, giftDto);
        }

        // PUT api/<GiftController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Put(int id, [FromBody] GiftDTO giftDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Manager updating gift {Id}", id);
            var updatedGift = await _giftBLL.Put(id, giftDto);
            return Ok(updatedGift);
        }
        // DELETE api/<GiftController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]

        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Manager deleting gift {Id}", id);
            await _giftBLL.Delete(id);
            return Ok(new { message = "Gift deleted successfully" });
        }
        [HttpPost("raffle/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> PerformRaffle(int id)
        {

            // 1. לוג תיעוד - מי המנהל שניסה לבצע הגרלה
            _logger.LogInformation("Manager initiating draw for Gift ID: {Id}", id);

            // קריאה לשירות שמבצע את האלגוריתם
            var giftWithWinner = await _giftBLL.PerformRaffle(id);

            _logger.LogInformation("Draw completed successfully. Winner: {WinnerName}", giftWithWinner.WinnerName);
            return Ok(giftWithWinner);
        }
        // דוח 1: רשימת זוכים - להורדה כקובץ
        [HttpGet("report")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> GetWinnersReport()
        {
            _logger.LogInformation("Manager generating Excel report");

            var fileBytes = await _giftBLL.GenerateReport();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Winners_Report_{DateTime.Now:yyyy-MM-dd}.xlsx"
            );
        }

        // בדיקת מערכת המיילים
        [HttpPost("test-email")]
        [Authorize(Roles = "Manager")] 
        public async Task<ActionResult> TestEmailSystem(string targetEmail)
        {
            if (string.IsNullOrEmpty(targetEmail))
                return BadRequest("נא להזין כתובת מייל לבדיקה");

            var subject = "בדיקת מערכת - מכירה סינית 🛠️";
            var body = @"
            <div style='direction: rtl; text-align: right; font-family: Arial;'>
                <h1>זהו מייל בדיקה</h1>
                <p>אם אתם רואים את ההודעה הזו - <b>הגדרות ה-SMTP תקינות!</b> ✅</p>
                <p>אפשר להמשיך להגרלה בראש שקט.</p>
            </div>";

            // קריאה ישירה לשירות המייל
            await _emailBLL.SendEmail(targetEmail, subject, body);

            return Ok($"המייל נשלח בהצלחה לכתובת: {targetEmail}");
        }

    }
}
