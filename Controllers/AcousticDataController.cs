using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Onyx.Models;
using Onyx.Models.Domain.AcousticData;
using Onyx.Repositories;
using Onyx.SignalR;
using System.Text.Json;

namespace Onyx.Controllers
{
    /// <summary>
    /// Controller with CRUD operations for Acoustic Data
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AcousticDataController : Controller
    {
        private readonly IMongoAcousticDataRepository _repository;
        private readonly IHubContext<DutUpdateHub> _hub;

        public AcousticDataController(
            IMongoAcousticDataRepository repository,
            IHubContext<DutUpdateHub> hubContext
        )
        {
            _repository = repository;
            _repository.Configure("acoustic_data", "acoustic_data");
            _hub = hubContext;
        }

        /// <summary>
        /// Retrieves multiple acoustic data records based on query parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters for filtering, sorting, and pagination.</param>
        /// <returns>A collection of acoustic data records matching the query parameters.</returns>
        [HttpGet]
        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> Getmany([FromQuery] QueryParams queryParams)
        {
            var result = await _repository.GetManyAsync(queryParams);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single acoustic data record by ID or serial number.
        /// </summary>
        /// <param name="idOrSerial">The ID or serial number of the acoustic data record to retrieve.</param>
        /// <returns>The requested acoustic data record.</returns>
        [HttpGet]
        [Route("{idOrSerial}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetOne([FromRoute] string idOrSerial)
        {
            var result = await _repository.GetOneAsync(idOrSerial);

            if (result == null)
                return NotFound($"Unit [{idOrSerial}] does not exist.");

            return Ok(result);
        }

        /// <summary>
        /// Returns the information to a client about how to connect to API via websockets to get data updates.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client subscribing to updates.</param>
        /// <returns>An instruction message for connecting via WebSockets; Unauthorized if invalid clientId.</returns>
        [HttpPost("subscribe")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> Subscribe([FromBody] string clientId)
        {
            if (!clientId.StartsWith("IE50") || string.IsNullOrEmpty(clientId))
                return Unauthorized();

            return Ok(new { message = "Connect via WebSockets to <api-address>/newdata-hub?clientId=<pc-name>&typeId=000000" });
        }

        /// <summary>
        /// Creates a new acoustic data record.
        /// </summary>
        /// <param name="unit">The acoustic data model representing the new record to create.</param>
        /// <returns>The newly created acoustic data record; StatusCode 409 if already exists.</returns>
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] NewAcousticDataModel unit)
        {
            var result = await _repository.CreateAsync(unit);

            if (result == null)
                return StatusCode(409, $"Unit {unit.DUT.SerialNr} already exists!");

            var group = $"G-{unit.DUT.TypeID}";
            //Console.WriteLine(group);

            await _hub.Clients.Group(group).SendAsync("NewDataAvailable", JsonSerializer.Serialize(result));

            return CreatedAtAction(nameof(Create), result);
        }

        /// <summary>
        /// Updates an existing acoustic data record.
        /// </summary>
        /// <param name="unit">The acoustic data model representing the updated record.</param>
        /// <returns>The updated acoustic data record.</returns>
        [HttpPut]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromBody] AcousticDataModel unit)
        {
            var result = await _repository.UpdateAsync(unit);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an acoustic data record by ID or serial number.
        /// </summary>
        /// <param name="idOrSerial">The ID or serial number of the acoustic data record to delete.</param>
        /// <returns>NoContent on successful deletion; BadRequest if not found.</returns>
        [HttpDelete]
        [Route("{idOrSerial}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] string idOrSerial)
        {
            var result = await _repository.GetOneAsync(idOrSerial);

            if (result == null)
                return BadRequest($"Unit [{idOrSerial}] does not exist.");

            await _repository.DeleteAsync(idOrSerial);
            return NoContent();
        }
    }
}
