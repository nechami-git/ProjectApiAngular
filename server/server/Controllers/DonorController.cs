using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using server.BLL;
using server.BLL.Intarfaces;
using server.DAL;
using server.Models;
using server.Models.DTO;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorController : ControllerBase
    {
        private readonly IDonorBLL _donorBLL;
        private readonly ILogger<DonorController> _logger;
        public DonorController(IDonorBLL donorBLL, ILogger<DonorController> logger)
        {
            this._donorBLL = donorBLL;
            this._logger = logger;
        }

        // GET: api/<DonorController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonorModel>>> Get([FromQuery] string? name, [FromQuery] string? email, [FromQuery] string? giftName)
        {
            _logger.LogInformation("Fetching all donors with gifts");
            var donors = await _donorBLL.GetDonors(name, email, giftName);
            return Ok(donors);
        }

        // GET api/<DonorController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DonorModel?>> Get(int id)
        {
            _logger.LogInformation($"Fetching donor with id: {id}");
            var donor = await _donorBLL.GetById(id);
            return Ok(donor);

        }

        // POST api/<DonorController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DonorDTO donorDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation($"Adding new donor: {donorDTO.FirstName} {donorDTO.LastName}");
            _logger.LogDebug($"debug - Donor details :{donorDTO.FirstName} - {donorDTO.LastName} ");
            await _donorBLL.Post(donorDTO);
            return CreatedAtAction(nameof(Get), new { id = donorDTO.Id }, donorDTO);
        }

        // PUT api/<DonorController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] DonorDTO donorDTO)
        {
            _logger.LogInformation($"Updating donor id: {id}");
            _logger.LogDebug($"put Donor: {donorDTO.FirstName} {donorDTO.LastName}");
            var updated = await _donorBLL.Put(id, donorDTO);
            return Ok(updated);

        }

        // DELETE api/<DonorController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation($"Deleting donor id: {id}");
            await _donorBLL.Delete(id);
            return Ok();
        }
    }
}
