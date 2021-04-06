using System.Threading.Tasks;
using rpg.DTOs.CharacterSkill;
using rpg.DTOs.Charecter;
using rpg.Models;

namespace rpg.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
         Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
    }
}