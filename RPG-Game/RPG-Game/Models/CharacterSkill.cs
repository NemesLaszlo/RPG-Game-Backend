using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Models
{
    public class CharacterSkill
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
