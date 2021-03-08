using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RPG_Game.Contracts;
using RPG_Game.Data;
using RPG_Game.Dtos.Character;
using RPG_Game.Dtos.CharacterSkill;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPG_Game.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterSkillService(IMapper mapper, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User.Id == GetUserId());
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                Skill skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
                if (skill == null)
                {
                    response.Success = false;
                    response.Message = "Skill not found";
                    return response;
                }

                CharacterSkill characterSkill = new CharacterSkill
                {
                    Character = character,
                    Skill = skill
                };

                await _context.CharacterSkills.AddAsync(characterSkill);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> DeleteCharacterSkill(DeleteCharacterSkillDto deleteCharacterSkill)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == deleteCharacterSkill.CharacterId && c.User.Id == GetUserId());
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                CharacterSkill characterSkill = await _context.CharacterSkills.FirstOrDefaultAsync(s => s.SkillId == deleteCharacterSkill.SkillId && s.CharacterId == deleteCharacterSkill.CharacterId);
                if (characterSkill == null)
                {
                    response.Success = false;
                    response.Message = "character skill not found.";
                    return response;
                }

                _context.CharacterSkills.Remove(characterSkill);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
