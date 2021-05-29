using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.DTOs.Fight;
using rpg.Models;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;

namespace rpg.Services.FightService
{

    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public FightService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _context.Characters.Include(c => c.User)
                .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill).FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                if (attacker.User.Id != GetUserId())
                {
                    response.success = false;
                    response.message = "You can not attack with a user that is not yours!";
                    return response;
                }

                Character opponent = await _context.Characters
                .Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (opponent == null)
                {
                    response.success = false;
                    response.message = "Opponent not found.";
                    return response;
                }

                CharacterSkill characterSkill = attacker.CharacterSkills.FirstOrDefault(cs => cs.SkillId == request.SkillId);

                if (characterSkill == null)
                {
                    response.success = false;
                    response.message = $"{attacker.Name} dosen't knoe taht skill.";
                    return response;
                }
                int damage = DoSkillAttack(attacker, opponent, characterSkill);
                if (opponent.HitPoint <= 0)
                    response.message = $"{opponent.Name} has been defeated!";

                _context.Update(opponent);
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoint,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoint,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }

            return response;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, CharacterSkill characterSkill)
        {
            int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoint -= damage;
            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();

            try
            {
                Character attacker = await _context.Characters
                .Include(c => c.Weapon).Include(c => c.User).FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                if (attacker.User.Id != GetUserId())
                {
                    response.success = false;
                    response.message = "You can not attack with a user that is not yours!";
                    return response;
                }

                Character opponent = await _context.Characters
                .Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (opponent == null)
                {
                    response.success = false;
                    response.message = "Opponent not found.";
                    return response;
                }

                int damage = DoWeaponAttck(attacker, opponent);
                if (opponent.HitPoint <= 0)
                    response.message = $"{opponent.Name} has been defeated!";

                _context.Update(opponent);
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoint,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoint,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }

            return response;
        }

        private static int DoWeaponAttck(Character attacker, Character opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoint -= damage;
            return damage;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };

            try
            {
                List<Character> characters = await _context.Characters.Include(c => c.Weapon)
                .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;
                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        Character opponent = opponents[new Random().Next(opponents.Count)];

                        int demage = 0;
                        string attackUsed = string.Empty;
                        bool useWeapon = new Random().Next(2) == 0;

                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            demage = DoWeaponAttck(attacker, opponent);
                        }
                        else
                        {
                            int randomSkill = new Random().Next(attacker.CharacterSkills.Count);
                            attackUsed = attacker.CharacterSkills[randomSkill].Skill.Name;
                            demage = DoSkillAttack(attacker, opponent, attacker.CharacterSkills[randomSkill]);
                        }

                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(demage >= 0 ? demage : 0)} demage.");

                        if (opponent.HitPoint <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeted");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoint} HP left!");
                            break;
                        }
                    }
                }
                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoint = 100;
                });

                _context.Characters.UpdateRange(characters);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                response.success = false;
                response.message = ex.Message;
            }

            return response;

        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            List<Character> characters = await _context.Characters.Where(c => c.Fights > 0)
            .OrderByDescending(c => c.Victories)
            .ThenBy(c => c.Defeats).ToListAsync();

            var response = new ServiceResponse<List<HighScoreDto>>
            {
                Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList()
            };

            return response;
        }
    }
}