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

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };
            try
            {
                List<Character> characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;
                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        Character opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            if (String.IsNullOrEmpty(attackUsed))
                            {
                                break;
                            }
                            damage = Utility.DoWeaponAttack(attacker, opponent);
                        }
                        else
                        {
                            if (attacker.CharacterSkills.Count == 0)
                            {
                                break;
                            }
                            int randomSkill = new Random().Next(attacker.CharacterSkills.Count);
                            attackUsed = attacker.CharacterSkills[randomSkill].Skill.Name;
                            damage = Utility.DoSkillAttack(attacker, opponent, attacker.CharacterSkills[randomSkill]);
                        }

                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                _context.Characters.UpdateRange(characters);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<FightResultDto>> DeathMachFight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };
            try
            {
                List<Character> characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                Dictionary<Character, string> FightingCharacters = new Dictionary<Character, string>();
                characters.ForEach(c =>
                {
                    FightingCharacters[c] = "Alive";
                });

                while (FightingCharacters.Count(c => c.Value.Equals("Alive")) != 1)
                {
                    foreach (KeyValuePair<Character, string> attacker in FightingCharacters)
                    {
                        List<Character> opponents = FightingCharacters.Where(c => c.Key.Id != attacker.Key.Id && c.Value.Equals("Alive")).Select(c => c.Key).ToList();
                        Character opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Key.Weapon.Name;
                            if (String.IsNullOrEmpty(attackUsed))
                            {
                                break;
                            }
                            damage = Utility.DoWeaponAttack(attacker.Key, opponent);
                        }
                        else
                        {
                            if (attacker.Key.CharacterSkills.Count == 0)
                            {
                                break;
                            }
                            int randomSkill = new Random().Next(attacker.Key.CharacterSkills.Count);
                            attackUsed = attacker.Key.CharacterSkills[randomSkill].Skill.Name;
                            damage = Utility.DoSkillAttack(attacker.Key, opponent, attacker.Key.CharacterSkills[randomSkill]);
                        }

                        response.Data.Log.Add($"{attacker.Key.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");

                        if (opponent.HitPoints <= 0)
                        {
                            FightingCharacters[opponent] = "Dead";
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated by {attacker.Key.Name}!");
                            break;
                        }
                    }
                }
                Character winner = FightingCharacters.FirstOrDefault(c => c.Value.Equals("Alive")).Key;
                response.Data.Log.Add($"{winner.Name} wins with {winner.HitPoints} HP left!");

                foreach (KeyValuePair<Character, string> character in FightingCharacters)
                {
                    if (character.Value.Equals("Alive"))
                    {
                        character.Key.Victories++;
                    }
                    character.Key.Fights++;
                    character.Key.HitPoints = 100;
                }
                List<Character> final = FightingCharacters.Keys.ToList();
                FightingCharacters.Clear();

                _context.Characters.UpdateRange(final);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighscore()
        {
            List<Character> characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();

            ServiceResponse<List<HighScoreDto>> response = new ServiceResponse<List<HighScoreDto>>
            {
                Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList()
            };
            return response;
        }

    }
}
