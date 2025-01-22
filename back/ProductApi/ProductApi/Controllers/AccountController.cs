using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly JsonUserRepository _userRepository;

        public AccountController()
        {
            _userRepository = new JsonUserRepository();
        }

        // POST /api/account
        // Création d'un utilisateur
        // J'ai fais uniquement une vérification basique sur le mail déjà existant pour ne pas avoir de doublons
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            var userExist = await _userRepository.GetUserByEmailAsync(user.Email);

            if (userExist != null)
            {
                return BadRequest("Cet email est déjà utilisé.");
            }

            await _userRepository.AddUserAsync(user);
            return Ok("Utilisateur créé avec succès !");
        }

        // GET /api/account
        // On récupère tous les utilisateurs
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
