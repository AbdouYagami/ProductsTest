using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Repositories;
using ProductApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ProductApi.Controllers
{
    // J'ai choisis de faire une gestion au plus simple de JWT avec une clé secrète pour signer les JWT
    [ApiController]
    [Route("/token")]
    public class TokenController : ControllerBase
    {
        private readonly JsonUserRepository _userRepository;
        private readonly string _secretKey = "eH$23!V@dKf7bJq#9tLmN*PzR4sT5xYA"; 

        public TokenController()
        {
            _userRepository = new JsonUserRepository();
        }

        // POST /token
        // Génère un token JWT pour un user créé au préalable
        [HttpPost]
        public async Task<IActionResult> GenerateToken([FromBody] LoginRequest loginRequest)
        {

            // Vérification que l'email et le mot de passe sont fournis
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email et mot de passe sont requis.");
            }

            // Voir si l'utilisateur existe
            var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);

            // Si l'utilisateur n'existe pas
            if (user == null)
            {
                return Unauthorized("L'utilisateur n'existe pas");
            }

            // Vérification du mot de passe
            if (user.Password != loginRequest.Password)
            {
                return Unauthorized("Mot de passe incorrect.");
            }
             

            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email et mot de passe sont requis.");
            }
             

            // Création du token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token valide pour 1 heure
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
}
