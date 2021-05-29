using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg.DTOs.Fight;
using rpg.Models;
using rpg.Services.FightService;

namespace rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }

        [HttpPost("weapon")]
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = await _fightService.WeaponAttack(request);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost("skill")]
        public async Task<IActionResult> SkilAttack(SkillAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = await _fightService.SkillAttack(request);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = await _fightService.Fight(request);
            if (response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}