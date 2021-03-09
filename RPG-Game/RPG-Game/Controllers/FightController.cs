using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPG_Game.Contracts;
using RPG_Game.Dtos.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Fight scene.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;
        private readonly ILoggerService _logger;

        public FightController(IFightService fightService, ILoggerService logger)
        {
            _fightService = fightService;
            _logger = logger;
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        /// <summary>
        /// Attack the opponent with weapon
        /// </summary>
        /// <param name="request">Weapon Attack Dto</param>
        /// <returns>With the attack result</returns>
        [HttpPost]
        [Route("Weapon")]
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto request)
        {
            string location = GetControllerActionNames();
            _logger.LogInfo($"{location}: Character with id: {request.AttackerId} attacked his/her opponent ( id: {request.OpponentId} ) with weapon.");
            return Ok(await _fightService.WeaponAttack(request));
        }

        /// <summary>
        /// Attack the opponent with skill
        /// </summary>
        /// <param name="request">Skill Attack Dto</param>
        /// <returns>With the attack result</returns>
        [HttpPost]
        [Route("Skill")]
        public async Task<IActionResult> SkillAttack(SkillAttackDto request)
        {
            string location = GetControllerActionNames();
            _logger.LogInfo($"{location}: Character with id: {request.AttackerId} attacked his/her opponent ( id: {request.OpponentId} ) with skill ( skill id: {request.SkillId} ).");
            return Ok(await _fightService.SkillAttack(request));
        }

        /// <summary>
        /// Start a fight, end with the first death
        /// </summary>
        /// <param name="request">Fight Request Dto</param>
        /// <returns>With the fight result</returns>
        [HttpPost]
        public async Task<IActionResult> Fight(FightRequestDto request)
        {
            string location = GetControllerActionNames();
            _logger.LogInfo($"{location}: Fight started");
            return Ok(await _fightService.Fight(request));
        }

        /// <summary>
        /// Start a deathmach
        /// </summary>
        /// <param name="request">Fight Request Dto</param>
        /// <returns>With the fight result</returns>
        [HttpPost]
        [Route("Deathmach")]
        public async Task<IActionResult> DeathMach(FightRequestDto request)
        {
            string location = GetControllerActionNames();
            _logger.LogInfo($"{location}: DeathMach started");
            return Ok(await _fightService.DeathMachFight(request));
        }


    }
}
