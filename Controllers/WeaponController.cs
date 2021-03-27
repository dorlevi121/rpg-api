using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg.DTOs.Charecter;
using rpg.DTOs.Weapon;
using rpg.Models;
using rpg.Services.WeaponService;

namespace rpg.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;
        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;

        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddWeaponDto newWeapon)
        {
            ServiceResponse<GetCharacterDto> respons = await _weaponService.AddWeapon(newWeapon);
            if (!respons.success)
            {
                return NotFound(respons);
            }
            return Ok(respons);
        }
    }
}