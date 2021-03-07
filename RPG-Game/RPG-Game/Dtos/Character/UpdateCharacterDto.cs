using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Dtos.Character
{
    public class UpdateCharacterDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int HitPoints { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Intelligence { get; set; }
        [Required]
        public RpgClass Class { get; set; }
    }
}
