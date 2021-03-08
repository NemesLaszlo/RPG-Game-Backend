using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPG_Game.Contracts;
using RPG_Game.Dtos.CharacterSkill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Characters' skills in the prg-game's database.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterSkillController : ControllerBase
    {
        private readonly ICharacterSkillService _characterSkillService;
        private readonly ILoggerService _logger;

        public CharacterSkillController(ICharacterSkillService characterSkillService, ILoggerService logger)
        {
            _characterSkillService = characterSkillService;
            _logger = logger;
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        /// <summary>
        /// Add a new skill to one of your character
        /// </summary>
        /// <param name="newCharacterSkill">Add Character Skill Dto</param>
        /// <returns>With the caracter info</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddCharacterSkill([FromBody] AddCharacterSkillDto newCharacterSkill)
        {
            string location = GetControllerActionNames();
            _logger.LogInfo($"{location}: Character with id: {newCharacterSkill.CharacterId} got a new skill and the skill id is {newCharacterSkill.SkillId} .");
            return Ok(await _characterSkillService.AddCharacterSkill(newCharacterSkill));
        }

        /// <summary>
        /// Delete one character's skill
        /// </summary>
        /// <param name="deleteCharacterSkill">Delete Character Skill Dto</param>
        /// <returns>With the caracter info</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCharacterSkill([FromBody] DeleteCharacterSkillDto deleteCharacterSkill)
        {
            string location = GetControllerActionNames();
            _logger.LogInfo($"{location}: Character with id: {deleteCharacterSkill.CharacterId} lose a skill and the skill id is {deleteCharacterSkill.SkillId} .");
            return Ok(await _characterSkillService.DeleteCharacterSkill(deleteCharacterSkill));
        }
    }
}
