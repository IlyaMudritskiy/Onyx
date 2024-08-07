using Microsoft.AspNetCore.Mvc;
using Onyx.Models.DTOs;
using Onyx.Repositories;

namespace Onyx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;

        public AuthController(IAuthRepository repository)
        {
            _repository = repository;
        }

        // POST: /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            var result = await _repository.RegisterUser(registerRequest);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // POST: /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var result = await _repository.LoginUser(loginRequest);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
