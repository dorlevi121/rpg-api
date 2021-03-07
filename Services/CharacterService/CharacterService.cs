using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.DTOs.Charecter;
using rpg.Models;

namespace rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private static List<Character> _characters = new List<Character> {
            new Character(),
            new Character{Id = 1, Name = "Dor"}
        };
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            Character character = _mapper.Map<Character>(newCharacter);
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            serviceResponse.Data = (_context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> dbCharacter = await _context.Characters.ToListAsync();
            serviceResponse.Data = (dbCharacter.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacter(int id)
        {
            ServiceResponse<GetCharacterDto> serviceCharacter = new ServiceResponse<GetCharacterDto>();
            Character dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceCharacter.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceCharacter;
        }

        public async Task<ServiceResponse<GetCharacterDto>> EditCharacter(AddCharacterDto character, int id)
        {
            ServiceResponse<GetCharacterDto> serviceCharacter = new ServiceResponse<GetCharacterDto>();

            try
            {
                Character curCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                curCharacter.Name = character.Name;
                curCharacter.Class = character.Class;
                curCharacter.Defense = character.Defense;
                curCharacter.HitPoint = character.HitPoint;
                curCharacter.Intelligence = character.Intelligence;
                curCharacter.Strength = character.Strength;

                _context.Characters.Update(curCharacter);
                await _context.SaveChangesAsync();

                serviceCharacter.Data = _mapper.Map<GetCharacterDto>(curCharacter);
            }
            catch (Exception ex)
            {
                serviceCharacter.success = false;
                serviceCharacter.message = ex.Message;
            }
            return serviceCharacter;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceCharacter = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                Character curCharacter = await _context.Characters.FirstAsync(c => c.Id == id);
                _context.Characters.Remove(curCharacter);
                await _context.SaveChangesAsync();

                serviceCharacter.Data = (_context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
            }
            catch (Exception ex)
            {
                serviceCharacter.success = false;
                serviceCharacter.message = ex.Message;
            }
            return serviceCharacter;
        }

    }
}