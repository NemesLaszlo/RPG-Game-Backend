using RPG_Game.Contracts;
using RPG_Game.Dtos.Character;
using RPG_Game.Dtos.CharacterSkill;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        public Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            throw new NotImplementedException();
        }
    }
}
