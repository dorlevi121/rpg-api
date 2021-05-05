using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.DTOs.Fight;
using rpg.Models;

namespace rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;

        public FightService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();

            try
            {
                Character attacker = await _context.Characters
                .Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                Character opponent = await _context.Characters
                .Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.OpponentId);

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