using Microsoft.AspNetCore.Mvc;
using ProductApi.Repositories;
using ProductApi.Models;
using System.Threading.Tasks;

namespace ProductApi.Controllers
{
    // Pareil que pou le Panier
    // J'avais deux options, sois à chaque requète on précisait l'email de l'utilisateur, sois je pars du principe que forcément pour accéder à sa liste d'envies
    // l'utilisateur doit etre connecté et donc pas besoin de mettre son mail dans la requète
    // A partir de son token d'authentification on retrouve l'utilisateur et on ajoute son mail dans les données de la liste des envies qui correspondent
    [Route("api/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly JsonWishlistRepository _wishlistRepository;
        private readonly JsonProductRepository _productRepository;

        public WishlistController(JsonWishlistRepository wishlistRepository, JsonProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
        }

        // GET
        // Récupérer la liste d'envies de l'utilisateur connecté
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userEmail = User.Identity.Name; // Récupère l'email de l'utilisateur connecté
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Vous devez être connecté pour accéder à votre liste d'envies.");
            }

            var wishlist = await _wishlistRepository.GetWishlistAsync(userEmail);

            if (wishlist == null || wishlist.Count == 0)
            {
                return NotFound("Liste d'envies non trouvée.");
            }

            return Ok(wishlist);
        }

        // POST
        // Ajouter un produit à la liste d'envies
        [HttpPost("add")]
        public async Task<IActionResult> AddProductToWishlist(int productId)
        {
            var userEmail = User.Identity.Name; // Récupère l'email de l'utilisateur connecté
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Vous devez être connecté pour ajouter un produit à votre liste d'envies.");
            }

            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
            {
                return NotFound("Produit non trouvé.");
            }

            await _wishlistRepository.AddProductToWishlistAsync(userEmail, product);
            return Ok("Produit ajouté à la liste d'envies.");
        }

        // DELETE
        // Supprimer un produit de la liste d'envies
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveProductFromWishlist(int productId)
        {
            var userEmail = User.Identity.Name; // Récupère l'email de l'utilisateur connecté
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Vous devez être connecté pour supprimer un produit de votre liste d'envies.");
            }

            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Produit non trouvé.");
            }

            var wishlist = await _wishlistRepository.GetWishlistAsync(userEmail);

            // Vérifier si le produit est dans la wishlist en fonction de l'ID
            var wishlistProduct = wishlist.FirstOrDefault(p => p.Id == productId);
            if (wishlistProduct == null)
            {
                return NotFound("Ce produit ne se trouve pas dans la liste d'envies.");
            }

            await _wishlistRepository.RemoveProductFromWishlistAsync(userEmail, productId);
            return Ok("LE produit a été supprimé de la liste d'envies !");
        }

        // DELETE
        // Vider la liste d'envies
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            var userEmail = User.Identity.Name; // Récupère l'email de l'utilisateur connecté
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Connectez vous pour pouvoir vider votre liste d'envies.");
            }

            await _wishlistRepository.ClearWishlistAsync(userEmail);
            return Ok("La liste des envies a été vidée !");
        }
    }
}