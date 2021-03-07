using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RPG_Game.Contracts;
using RPG_Game.Data;
using RPG_Game.Dtos.Character;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CharacterService(IMapper mapper, ApplicationDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = (dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            Character dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);

            return serviceResponse;
        }

        public async Task<bool> CreateCharacter(CreateCharacterDto newCharacter)
        {
            Character character = _mapper.Map<Character>(newCharacter);
            await _context.Characters.AddAsync(character);
            return await Save();
        }

        public async Task<bool> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            Character character = _mapper.Map<Character>(updatedCharacter);
            _context.Characters.Update(character);
            return await Save();
        }

        public async Task<bool> DeleteCharacter(int id)
        {
            Character character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            _context.Characters.Remove(character);
            return await Save();
        }

        public async Task<bool> IsExists(int id)
        {
            return await _context.Characters.AnyAsync(q => q.Id == id);
        }

        public async Task<bool> Save()
        {
            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

    }
}
