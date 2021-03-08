using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPG_Game.Contracts;
using RPG_Game.Dtos.User;
using RPG_Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPG_Game.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Users in the prg-game's database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILoggerService _logger;

        public AuthController(IAuthService authService, ILoggerService logger)
        {
            _authService = authService;
            _logger = logger;
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        /// <summary>
        /// User Registration
        /// </summary>
        /// <param name="request">User Register Dto</param>
        /// <returns>Response with the id of the new user</returns>
        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            string location = GetControllerActionNames();
            ServiceResponse<int> response = await _authService.Register(
                new User { Username = request.Username }, request.Password);
            if (!response.Success)
            {
                _logger.LogWarn($"{location}: {response.Message} register operation failed.");
                return BadRequest(response);
            }
            _logger.LogInfo($"{location}: User with id: {response.Data} successfully registered.");
            return Ok(response);
        }

        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="request">User Login Dto</param>
        /// <returns>With the token</returns>
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            string location = GetControllerActionNames();
            ServiceResponse<string> response = await _authService.Login(
                request.Username, request.Password);
            if (!response.Success)
            {
                _logger.LogWarn($"{location}: User {request.Username} - {response.Message} login operation failed.");
                return BadRequest(response);
            }
            _logger.LogInfo($"{location}: User {request.Username} successfully logged in.");
            return Ok(response);
        }
    }
}
