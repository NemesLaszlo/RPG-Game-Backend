using RPG_Game.Dtos.Character;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Contracts
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();
        Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);
        Task<bool> CreateCharacter(CreateCharacterDto newCharacter);
        Task<bool> UpdateCharacter(int id, UpdateCharacterDto updatedCharacter);
        Task<bool> DeleteCharacter(int id);
        Task<bool> Save();
        Task<bool> IsExists(int id);
    }
}
