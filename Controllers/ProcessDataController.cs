using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Onyx.Models;
using Onyx.Models.Domain;
using Onyx.Repositories;
using Onyx.SignalR;
using System.Text.Json;

namespace Onyx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessDataController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProcessDataRepository _repository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IHubContext<DutUpdateHub> _hub;
        private readonly ILogger<ProcessDataController> _log;

        public ProcessDataController(
            IMapper mapper, 
            IProcessDataRepository repository, 
            IHttpContextAccessor httpContextAccessor, 
            IHubContext<DutUpdateHub> hubContext,
            ILogger<ProcessDataController> logger
        )
        {
            _mapper = mapper;
            _repository = repository;
            _httpContext = httpContextAccessor;
            _hub = hubContext;
            _log = logger;
        }

        // GET ProcessData
        // GET: api/ProcessData?filterField=DUT.Track&filterValue=1&filterField=DUT.Press&filterValue=1&sortBy=DUT.Serial&isAscending=true
        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetMany([FromQuery] QueryParams queryParams)
        {
            var result = await _repository.GetManyAsync(queryParams);
            return Ok(result);
        }

        // GET One ProcessData unit by serial_nr or Id
        // GET: /api/ProcessData/240625034630111 or 66a7905ebc7be3ef2512ad6a
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

        [HttpPost("subscribe")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> Subscribe([FromBody] string clientId)
        {
            if (!clientId.StartsWith("IE50") || string.IsNullOrEmpty(clientId))
                return Unauthorized();

            return Ok(new { message = "Connect via WebSockets to <api-address>/processdata-hub?clientId=<pc-name>" });
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] NewProcessDataModel unit)
        {
            var result = await _repository.CreateAsync(unit);

            if (result == null)
                return StatusCode(409, $"Unit {unit.DUT.SerialNr} already exists!");

            // Send new DUT to connected clients in background
            _hub.Clients.All.SendAsync("NewDataAvailable", JsonSerializer.Serialize(result));

            return CreatedAtAction(nameof(Create), result);
        }

        [HttpPut]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateById([FromBody] ProcessDataModel unit)
        {
            var result = await _repository.UpdateAsync(unit);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{idOrSerial}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] string idOrSerial)
        {
            var result = await _repository.GetOneAsync(idOrSerial);

            if (result == null)
                return NotFound($"Unit [{idOrSerial}] does not exist.");

            await _repository.DeleteAsync(idOrSerial);
            return NoContent();
        }
    }
}
