using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Dtos.Weapon
{
    public class DeleteWeaponDto
    {
        public int CharacterId { get; set; }
        public int WeaponId { get; set; }
    }
}
