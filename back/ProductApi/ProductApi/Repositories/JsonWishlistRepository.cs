using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProductApi.Models;
using Newtonsoft.Json;

namespace ProductApi.Repositories
{
    // Utilisation de repository pour gérer les données via JSON et faciliter la gestion en SQL si jamais c'utilisé plus tard
    public class JsonWishlistRepository
    {
        private readonly string _filePath = "wishlist.json"; // fichier utilisé pour test

         
        public async Task<List<Product>> GetWishlistAsync(string userEmail)
        {
            var wishlists = await LoadWishlistDataAsync();
            return wishlists.FirstOrDefault(w => w.UserEmail == userEmail)?.Products ?? new List<Product>();
        }
        // Ajout d'un produit dans  la liste des envies
        public async Task AddProductToWishlistAsync(string userEmail, Product product)
        {
            var wishlists = await LoadWishlistDataAsync();
            var userWishlist = wishlists.FirstOrDefault(w => w.UserEmail == userEmail);
            if (userWishlist == null)
            {
                userWishlist = new Wishlist { UserEmail = userEmail, Products = new List<Product>() };
                wishlists.Add(userWishlist);
            }
            userWishlist.Products.Add(product);
            await SaveWishlistDataAsync(wishlists);
        }
        // Le supprimer de la liste
        public async Task RemoveProductFromWishlistAsync(string userEmail, int productId)
        {
            var wishlists = await LoadWishlistDataAsync();
            var userWishlist = wishlists.FirstOrDefault(w => w.UserEmail == userEmail);
            if (userWishlist != null)
            {
                var product = userWishlist.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    userWishlist.Products.Remove(product);
                    await SaveWishlistDataAsync(wishlists);
                }
            }
        }
        // Vider la liste à 0
        public async Task ClearWishlistAsync(string userEmail)
        {
            var wishlists = await LoadWishlistDataAsync();
            var userWishlist = wishlists.FirstOrDefault(w => w.UserEmail == userEmail);
            if (userWishlist != null)
            {
                userWishlist.Products.Clear();
                await SaveWishlistDataAsync(wishlists);
            }
        }
        // Recup des données via le fichier json
        private async Task<List<Wishlist>> LoadWishlistDataAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Wishlist>();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonConvert.DeserializeObject<List<Wishlist>>(json) ?? new List<Wishlist>();
        }

        // Sauvegarder les données dans le fichier json
        private async Task SaveWishlistDataAsync(List<Wishlist> wishlists)
        {
            var json = JsonConvert.SerializeObject(wishlists, Formatting.Indented);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }

}