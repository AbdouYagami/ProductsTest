namespace ProductApi.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    // Je pouvais choisir d'autre option pour optimiser le code mais j'ai choisis une bonne sturcutre avec le modèle et le controller pour le Cart et la liste des envies
    public class Cart
    {
        [Required(ErrorMessage = "L'email de l'utilisateur est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Le panier doit contenir au moins un produit.")]
        public List<Product> Products { get; set; } = new List<Product>();
    }
}