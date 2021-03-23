using System.Collections.Generic;
using System.Threading.Tasks;
using rpg.DTOs.Charecter;
using rpg.Models;

namespace rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();
        Task<ServiceResponse<GetCharacterDto>> GetCharacter(int id);
        Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);
        Task<ServiceResponse<GetCharacterDto>> EditCharacter(AddCharacterDto character, int id);
        Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);

    }
}