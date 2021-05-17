using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.DTOs.Fight;
using rpg.Models;
using System.Linq;

namespace rpg.Services.FightService
{

    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FightService(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
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
                int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
                damage -= new Random().Next(opponent.Defense);

                if (damage > 0)
                    opponent.HitPoint -= damage;
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

                int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
                damage -= new Random().Next(opponent.Defense);

                if (damage > 0)
                    opponent.HitPoint -= damage;
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
    }
}