using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rpg.Data;
using rpg.DTOs.User;
using rpg.Models;

namespace rpg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] NewUserDto user)
        {
            ServiceResponse<int> response = await _authRepository.Register(new User { Username = user.Username }, user.Password);
            if (!response.success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDetails)
        {
            ServiceResponse<string> response = await _authRepository.Login(userDetails.Username, userDetails.Password);
            if (!response.success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
    }
}