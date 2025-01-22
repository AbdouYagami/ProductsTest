using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models
{
    // J'ai choisis que les 4 champs soient obligatoire avec des annotations / message d'erreur si ce n'est pas respecté
    // Ainsi que des messages en + pour des erreurs classique comme le format du mail ou la taille du mdp
    public class User
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire.")]
        public string Username { get; set; } 

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public string Firstname { get; set; } 

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Le format de l'email est invalide.")]
        public string Email { get; set; } 

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string Password { get; set; } 
    }
}
