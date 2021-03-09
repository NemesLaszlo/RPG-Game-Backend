using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPG_Game.Contracts;
using RPG_Game.Dtos.Character;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Characters in the prg-game's database.
    /// </summary>
    [Authorize(Roles = "Player,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        private readonly ILoggerService _logger;

        public CharactersController(ICharacterService characterService, ILoggerService logger)
        {
            _characterService = characterService;
            _logger = logger;
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, message);
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        /// <summary>
        /// Get all Characters
        /// </summary>
        /// <returns>List of Characters</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCharacters()
        {
            string location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Successfully got all Characters");
                return Ok(await _characterService.GetAllCharacters());
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Get character by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>One specific character by id</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCharacter(int id)
        {
            string location = GetControllerActionNames();
            try
            {
                return Ok(await _characterService.GetCharacterById(id));
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Create a new Character
        /// </summary>
        /// <param name="newCharacter">Create Character DTO</param>
        /// <returns>Name of the new Character</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCharacter([FromBody] CreateCharacterDto newCharacter)
        {
            string location = GetControllerActionNames();
            try
            {
                if (newCharacter == null)
                {
                    _logger.LogWarn($"{location}: Empty Request was submitted.");
                    return BadRequest(ModelState);
                }
                bool isSuccess = await _characterService.CreateCharacter(newCharacter);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Character creation failed.");
                }
                _logger.LogInfo($"{location}: Character created successfully.");
                return Created("Created", newCharacter.Name);

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Update a Character by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedCharacter">Update Character DTO</param>
        /// <returns>NoContent</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCharacter(int id, [FromBody] UpdateCharacterDto updatedCharacter)
        {
            string location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Character with id: {id} Update Attempted");
                if (id < 1 || updatedCharacter == null || !Enum.IsDefined(typeof(RpgClass), updatedCharacter.Class))
                {
                    _logger.LogWarn($"{location}: Character Update failed with bad data");
                    return BadRequest();
                }
                bool isExists = await _characterService.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location}: Character with id: {id} was not found");
                    return NotFound();
                }
                bool isSuccess = await _characterService.UpdateCharacter(id, updatedCharacter);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Update operation failed.");
                }
                _logger.LogWarn($"{location}: Character with id: {id} successfully updated.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Delete a Character by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>NoContent</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            string location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Character with id: {id} Delete Attempted");
                if (id < 1)
                {
                    _logger.LogWarn($"{location}: Character Delete failed with bad data");
                    return BadRequest();
                }
                bool isExists = await _characterService.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location}: Character with id: {id} was not found");
                    return NotFound();
                }
                var isSuccess = await _characterService.DeleteCharacter(id);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Character delete failed");
                }
                _logger.LogWarn($"{location}: Character with id: {id} successfully deleted");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

    }
}
