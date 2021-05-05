using System.Threading.Tasks;
using rpg.DTOs.Fight;
using rpg.Models;

namespace rpg.Services.FightService
{
    public interface IFightService
    {
         Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request);
    }
}