using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RPG_Game.Contracts;
using RPG_Game.Data;
using RPG_Game.Dtos.Character;
using RPG_Game.Dtos.Weapon;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RPG_Game.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeaponService(IMapper mapper, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.User.Id == GetUserId());
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }
                Weapon weapon = new Weapon
                {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };

                await _context.Weapons.AddAsync(weapon);
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

        public async Task<ServiceResponse<GetCharacterDto>> ChangeWeapon(AddWeaponDto newWeapon)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.User.Id == GetUserId());
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                Weapon weapon = await _context.Weapons.FirstOrDefaultAsync(w => w.CharacterId == newWeapon.CharacterId);
                if (weapon != null)
                {
                    weapon.Name = newWeapon.Name;
                    weapon.Damage = newWeapon.Damage;
                    weapon.Character = character;

                    _context.Weapons.Update(weapon);
                    await _context.SaveChangesAsync();
                }
                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> DeleteWeapon(DeleteWeaponDto delWeapon)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == delWeapon.CharacterId && c.User.Id == GetUserId());
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                Weapon weapon = await _context.Weapons.FirstOrDefaultAsync(w => w.Id == delWeapon.WeaponId);
                if (weapon == null)
                {
                    response.Success = false;
                    response.Message = "Character's weapon not found.";
                    return response;
                }

                _context.Weapons.Remove(weapon);
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
