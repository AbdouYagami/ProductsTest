using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Le format de l'email est invalide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")] 
        public string Password { get; set; }
    }
}
