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
            ServiceResponse<int> respone = await _authRepository.Register(new User { Username = user.Username }, user.Password);
            if (!respone.success)
            {
                return BadRequest(respone);
            }
            return Ok(respone);
        }

    }
}