using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Dtos.Fight
{
    public class WeaponAttackDto
    {
        public int AttackerId { get; set; }
        public int OpponentId { get; set; }
    }
}
