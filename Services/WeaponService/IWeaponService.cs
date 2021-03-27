using System.Threading.Tasks;
using rpg.DTOs.Charecter;
using rpg.DTOs.Weapon;
using rpg.Models;

namespace rpg.Services.WeaponService
{
    public interface IWeaponService
    {
         Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newweapon);
    }
}