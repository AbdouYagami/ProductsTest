using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProductApi.Models;
using Newtonsoft.Json;

namespace ProductApi.Repositories
{
    // Utilisation de repository pour gérer les données via JSON et faciliter la gestion en SQL si jamais c'utilisé plus tard
    public class JsonCartRepository
    {
        private readonly string _filePath = "cart.json"; // fichier qui sert de test qui va garder les données liés au panier

        public async Task<List<Product>> GetCartAsync(string userEmail)
        {
            var carts = await LoadCartDataAsync();
            return carts.FirstOrDefault(c => c.UserEmail == userEmail)?.Products ?? new List<Product>();
        }

        // Ajout d'un produit dans le panier
        public async Task AddProductToCartAsync(string userEmail, Product product)
        {
            var carts = await LoadCartDataAsync();
            var userCart = carts.FirstOrDefault(c => c.UserEmail == userEmail);

            // Si le panier de l'utilisateur n'existe pas on crée une liste vide avant
            if (userCart == null)
            {
                userCart = new Cart { UserEmail = userEmail, Products = new List<Product>() };
                carts.Add(userCart);
            }
            userCart.Products.Add(product);
            await SaveCartDataAsync(carts);
        }

        // Supprimer un élement du panier
        public async Task RemoveProductFromCartAsync(string userEmail, int productId)
        {
            var carts = await LoadCartDataAsync();
            var userCart = carts.FirstOrDefault(c => c.UserEmail == userEmail);

            // Si le panier est vide il n'y a rien a supprimé

            if (userCart != null)
            {
                var product = userCart.Products.FirstOrDefault(p => p.Id == productId);
                // On vérifie que le produit qu'on veut supprimer existe
                if (product != null)
                {
                    userCart.Products.Remove(product);
                    await SaveCartDataAsync(carts);
                }
            }
        }

        // méthode utilisé pour vider le panier, on vide la liste
        public async Task ClearCartAsync(string userEmail)
        {
            var carts = await LoadCartDataAsync();
            var userCart = carts.FirstOrDefault(c => c.UserEmail == userEmail);
            if (userCart != null)
            {
                userCart.Products.Clear();
                await SaveCartDataAsync(carts);
            }
        }

        // Récupération de toutes les données du fichier json
        private async Task<List<Cart>> LoadCartDataAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Cart>();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonConvert.DeserializeObject<List<Cart>>(json) ?? new List<Cart>();
        }

        // Sauvegarde des données dans le fichier json
        private async Task SaveCartDataAsync(List<Cart> carts)
        {
            var json = JsonConvert.SerializeObject(carts, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
     
}