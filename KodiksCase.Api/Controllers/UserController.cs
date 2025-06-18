using KodiksCase.Application.Managers;
using KodiksCase.Shared.DTOs.Requests;
using KodiksCase.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace KodiksCase.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IServiceManager serviceManager;

        public UserController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Authenticates a user with the provided credentials and returns a JWT token on success.
        /// </summary>
        /// <param name="requestDto">The login credentials including username and password.</param>
        /// <returns>
        /// Returns 200 OK with the JWT token if authentication is successful.
        /// Returns 400 Bad Request if the request data is invalid or an error occurs.
        /// Returns 401 Unauthorized if the credentials are incorrect.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid request data."));
            try
            {
                var user = await serviceManager.UserService.ValidateUserAsync(requestDto);
                if (user == null)
                    return Unauthorized(ApiResponse<string>.Fail("Username or password incorrect."));

                var token = serviceManager.TokenService.GenerateToken(user.Id.ToString());
                return Ok(ApiResponse<string>.SuccessResponse(token, "Login successful."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="requestDto">The user registration information.</param>
        /// <returns>
        /// Returns 200 OK if the user is created successfully.
        /// Returns 400 Bad Request if the request data is invalid or an error occurs.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Invalid request data."));
            try
            {
                await serviceManager.UserService.CreateUserAsync(requestDto);
                return Ok(ApiResponse<string>.SuccessResponse("User created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a list of all registered users.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with the list of users.
        /// Returns 400 Bad Request if an error occurs.
        /// </returns>
        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            try
            {
                var users = await serviceManager.UserService.GetUsers();
                return Ok(ApiResponse<object>.SuccessResponse(users));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }
    }
}
