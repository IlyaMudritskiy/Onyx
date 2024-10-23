using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onyx.Models.DTOs;
using Onyx.Repositories;

namespace Onyx.Controllers
{
    /// <summary>
    /// Manages authentication operations, including user registration and login.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;

        public AuthController(IAuthRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerRequest">The request object containing registration details.</param>
        /// <returns>A status code indicating success or failure, along with a message describing the result.</returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            var result = await _repository.RegisterUser(registerRequest);

            if (!result.Success)
                return StatusCode(result.HttpCode, result.Message);

            return StatusCode(result.HttpCode, result.Message);
        }

        /// <summary>
        /// Authenticates a user and logs them in.
        /// </summary>
        /// <param name="loginRequest">The request object containing login credentials.</param>
        /// <returns>A status code indicating success or failure; upon success, returns authentication data (JWT).</returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var result = await _repository.LoginUser(loginRequest);

            if (!result.Success)
                return StatusCode(result.HttpCode, result.Message);

            return StatusCode(result.HttpCode, result.Data);
        }

        [HttpGet]
        [Authorize]
        [Route("Ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok();
        }
    }
}
