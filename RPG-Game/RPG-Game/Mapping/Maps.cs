﻿using AutoMapper;
using RPG_Game.Dtos.Character;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Mapping
{
    public class Maps: Profile
    {
        public Maps()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<CreateCharacterDto, Character>();
            CreateMap<UpdateCharacterDto, Character>();
        }
    }
}
