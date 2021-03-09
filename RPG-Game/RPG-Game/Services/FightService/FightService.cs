using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RPG_Game.Contracts;
using RPG_Game.Data;
using RPG_Game.Dtos.Fight;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPG_Game.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FightService(IMapper mapper, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId && c.User.Id == GetUserId());
                if (attacker == null)
                {
                    response.Success = false;
                    response.Message = "Attacker not found or not yours.";
                    return response;
                }

                Character opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);
                if (opponent == null)
                {
                    response.Success = false;
                    response.Message = "Opponent not found.";
                    return response;
                }

                if (attacker.HitPoints <= 0)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} has been defeated can't attack!";
                    return response;
                }
                if (opponent.HitPoints <= 0)
                {
                    response.Success = false;
                    response.Message = $"{opponent.Name} has been defeated can't fight more!";
                    return response;
                }
                int damage = Utility.DoWeaponAttack(attacker, opponent);
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated!";
                }

                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _context.Characters
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId && c.User.Id == GetUserId());
                if (attacker == null)
                {
                    response.Success = false;
                    response.Message = "Attacker not found or not yours.";
                    return response;
                }

                CharacterSkill characterSkill =
                    attacker.CharacterSkills.FirstOrDefault(cs => cs.Skill.Id == request.SkillId);
                if (characterSkill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill.";
                    return response;
                }

                Character opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);
                if (opponent == null)
                {
                    response.Success = false;
                    response.Message = "Opponent not found.";
                    return response;
                }

                if (attacker.HitPoints <= 0)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} has been defeated can't attack!";
                    return response;
                }
                if (opponent.HitPoints <= 0)
                {
                    response.Success = false;
                    response.Message = $"{opponent.Name} has been defeated can't fight more!";
                    return response;
                }
                int damage = Utility.DoSkillAttack(attacker, opponent, characterSkill);
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated!";
                }

                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<HighScoreDto>>> GetHighscore()
        {
            throw new NotImplementedException();
        }

    }
}
