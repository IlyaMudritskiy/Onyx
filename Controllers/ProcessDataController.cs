using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Onyx.Models;
using Onyx.Models.Domain.ProcessData;
using Onyx.Repositories;
using Onyx.SignalR;
using System.Text.Json;

namespace Onyx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessDataController : ControllerBase
    {
        private readonly IMongoProcessDataRepository _repository;
        private readonly IHubContext<DutUpdateHub> _hub;

        public ProcessDataController(
            IMongoProcessDataRepository repository,
            IHubContext<DutUpdateHub> hubContext
        )
        {
            _repository = repository;
            _repository.Configure("process_data", "process_data");
            _hub = hubContext;
        }

        /// <summary>
        /// Retrieves multiple records based on the provided query parameters.
        /// </summary>
        /// <remarks>
        /// This method is accessible only to users authorized with the 'Reader' role.
        /// It performs an asynchronous operation to fetch data from the underlying repository.
        /// </remarks>
        /// <param name="queryParams">The query parameters used to filter the records.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation, which includes the fetched records upon success.</returns>
        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetMany([FromQuery] QueryParams queryParams)
        {
            var result = await _repository.GetManyAsync(queryParams);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single record by its ID or serial number.
        /// </summary>
        /// <remarks>
        /// This method requires the caller to be authorized with the 'Reader' role.
        /// It supports retrieval via either an ID or a serial number, both of which are part of the route.
        /// If the requested record does not exist, a 404 Not Found status code is returned.
        /// </remarks>
        /// <param name="idOrSerial">The ID or serial number of the record to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation. 
        /// Upon success, it returns the retrieved record wrapped in an <see cref="OkObjectResult"/>.
        /// If the record is not found, a <see cref="NotFoundObjectResult"/> is returned instead.</returns>
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
        /// Subscribes a client to receive updates via WebSockets.
        /// </summary>
        /// <remarks>
        /// This method is accessible only to users authorized with the 'Reader' role.
        /// It validates the client ID against a predefined pattern and returns a WebSocket connection URL.
        /// </remarks>
        /// <param name="clientId">The client ID (PC name) from which client is trying to subscribe.</param>
        /// <returns>An <see cref="IActionResult"/> with information how to subscribe for updates.</returns>
        [HttpPost("subscribe")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> Subscribe([FromBody] string clientId)
        {
            if (!clientId.StartsWith("IE50") || string.IsNullOrEmpty(clientId))
                return Unauthorized();

            return Ok(new { message = "Connect via WebSockets to <api-address>/newdata-hub?clientId=<pc-name>" });
        }

        /// <summary>
        /// Creates a new record in the database.
        /// </summary>
        /// <remarks>
        /// This method is accessible only to users authorized with the 'Writer' role.
        /// It creates a new record in the repository and broadcasts the new data to all connected clients.
        /// </remarks>
        /// <param name="unit">The new record to create.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the creation operation.</returns>
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] NewProcessDataModel unit)
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
        /// Updates an existing record by its ID.
        /// </summary>
        /// <remarks>
        /// This method is accessible only to users authorized with the 'Writer' role.
        /// It updates the specified record in the repository.
        /// </remarks>
        /// <param name="unit">The updated record.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the update operation.</returns>
        [HttpPut]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromBody] ProcessDataModel unit)
        {
            var result = await _repository.UpdateAsync(unit);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a record by its ID or serial number.
        /// </summary>
        /// <remarks>
        /// This method is accessible only to users authorized with the 'Writer' role.
        /// It deletes the specified record from the repository.
        /// </remarks>
        /// <param name="idOrSerial">The ID or serial number of the record to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the deletion operation.</returns>
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
