using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPG_Game.Contracts;
using RPG_Game.Dtos.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Weapons in the prg-game's database.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;
        private readonly ILoggerService _logger;

        public WeaponController(IWeaponService weaponService, ILoggerService logger)
        {
            _weaponService = weaponService;
            _logger = logger;
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        /// <summary>
        /// Add a weapon to your character
        /// </summary>
        /// <param name="newWeapon">Add weapon Dto</param>
        /// <returns>With the character with recentry the added weapon</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddWeapon([FromBody] AddWeaponDto newWeapon)
        {
            string location = GetControllerActionNames();
            _logger.LogInfo($"{location}: Character with id: {newWeapon.CharacterId} got a {newWeapon.Name} weapon.");
            return Ok(await _weaponService.AddWeapon(newWeapon));
        }

        /// <summary>
        /// Change the weapon.
        /// </summary>
        /// <param name="newWeapon">Add weapon Dto</param>
        /// <returns>With the character with recentry the added (chenge to new) weapon</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeWeapon([FromBody] AddWeaponDto newWeapon)
        {
            string location = GetControllerActionNames();
            _logger.LogInfo($"{location}: Character with id: {newWeapon.CharacterId} switch to a {newWeapon.Name} weapon.");
            return Ok(await _weaponService.ChangeWeapon(newWeapon));
        }
    }
}
