using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.DTOs.Charecter;
using rpg.Models;

namespace rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            Character character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == this.GetUserId());
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            serviceResponse.Data = (_context.Characters.Where(c => c.User.Id == this.GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> dbCharacter = await _context.Characters.Where(c => c.User.Id == this.GetUserId()).ToListAsync();
            serviceResponse.Data = (dbCharacter.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacter(int id)
        {
            ServiceResponse<GetCharacterDto> serviceCharacter = new ServiceResponse<GetCharacterDto>();
            Character dbCharacter = await _context.Characters
            .Include(c => c.Weapon).Include(c => c.CharacterSkills).ThenInclude(c => c.Skill)
            .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
            serviceCharacter.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceCharacter;
        }

        public async Task<ServiceResponse<GetCharacterDto>> EditCharacter(AddCharacterDto character, int id)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();

            try
            {
                Character curCharacter = await _context.Characters.Include(u => u.User).FirstOrDefaultAsync(c => c.Id == id && c.User.Id == this.GetUserId());
                if (curCharacter != null)
                {
                    curCharacter.Name = character.Name;
                    curCharacter.Class = character.Class;
                    curCharacter.Defense = character.Defense;
                    curCharacter.HitPoint = character.HitPoint;
                    curCharacter.Intelligence = character.Intelligence;
                    curCharacter.Strength = character.Strength;

                    _context.Characters.Update(curCharacter);
                    await _context.SaveChangesAsync();

                    serviceResponse.Data = _mapper.Map<GetCharacterDto>(curCharacter);
                }
                else
                {
                    serviceResponse.success = false;
                    serviceResponse.message = "Character not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                Character curCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == this.GetUserId());
                if (curCharacter != null)
                {
                    _context.Characters.Remove(curCharacter);
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = (_context.Characters.Where(c => c.User.Id == this.GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
                }
                else
                {
                    serviceResponse.success = false;
                    serviceResponse.message = "Character not found.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.success = false;
                serviceResponse.message = ex.Message;
            }
            return serviceResponse;
        }

    }
}