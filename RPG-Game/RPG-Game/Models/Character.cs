using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Models
{
    public class Character
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public int HitPoints { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Intelligence { get; set; }
        [Required]
        public RpgClass Class { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Weapon Weapon { get; set; }
    }
}
