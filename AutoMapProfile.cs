using AutoMapper;
using rpg.DTOs.Charecter;
using rpg.Models;

namespace rpg
{
    public class AutoMapProfile : Profile
    {
        public AutoMapProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
        }

    }
}