using System.Linq;
using AutoMapper;
using rpg.DTOs.Charecter;
using rpg.DTOs.Skill;
using rpg.DTOs.Weapon;
using rpg.Models;

namespace rpg
{
    public class AutoMapProfile : Profile
    {
        public AutoMapProfile()
        {
            CreateMap<Character, GetCharacterDto>()
            .ForMember(c => c.Skills, c => c.MapFrom(cs => cs.CharacterSkills.Select(cs => cs.Skill)));
            CreateMap<AddCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
        }

    }
}