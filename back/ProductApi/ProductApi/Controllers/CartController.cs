using Microsoft.AspNetCore.Mvc;
using ProductApi.Repositories;
using ProductApi.Models;
using System.Threading.Tasks;

namespace ProductApi.Controllers
{
    // J'avais deux options, sois à chaque requète on précisait l'email de l'utilisateur, sois je pars du principe que forcément pour accéder à son panier
    // l'utilisateur doit etre connecté et donc pas besoin de mettre son mail dans la requète
    // A partir de son token d'authentification on retrouve l'utilisateur et on ajoute son mail dans les données du panier correspondant
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly JsonCartRepository _cartRepository;
        private readonly JsonProductRepository _productRepository;

        public CartController(JsonCartRepository cartRepository, JsonProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        // GET
        // Récupérer le panier de l'utilisateur qui s'est connecté
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userEmail = User.Identity.Name; // Récupère l'email de l'utilisateur connecté
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Vous devez être connecté pour accéder à votre panier. Veuillez vous connecter !!");
            }

            var cart = await _cartRepository.GetCartAsync(userEmail);

            // Dans le cas ou l'utilisateur est connecté mais que son panier est vide !
            if (cart == null || cart.Count == 0)
            {
                return NotFound("Panier non trouvé.");
            }

            return Ok(cart);
        }

        // POST
        // Ajouter un produit dans le panier
        // Petite remarque, sur le panier je suis partis du principe qu'on pouvait avoir plusieurs "meme" produit et donc les doublons sont acceptés, bien sur dans une meilleure gestion il faudrait ajouter un champ supplémentaire pour gérer au mieux ce cas là

        [HttpPost("add")]
        public async Task<IActionResult> AddProductToCart(int productId)
        {
            var userEmail = User.Identity.Name;  
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Vous devez être connecté pour ajouter un produit à votre panier. Veuillez vous connecter !!");
            }

            var product = await _productRepository.GetProductByIdAsync(productId);

            // Si l'utilisateur souhaite ajouter un produit qui n'existe pas 
            if (product == null)
            {
                return NotFound("Produit non trouvé.");
            }

            await _cartRepository.AddProductToCartAsync(userEmail, product);
            return Ok("Produit ajouté au panier.");
        }

        // DELETE
        // Supprimer un produit du panier
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveProductFromCart(int productId)
        {
            var userEmail = User.Identity.Name;  
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Vous devez être connecté pour supprimer un produit de votre panier. Veuillez vous connecter !!");
            }
            // Si le produit existe pas
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Produit non trouvé.");
            }

            var cart = await _cartRepository.GetCartAsync(userEmail);

            // Si le produit existe, il faut vérifier si il se trouve dans le paneir
            var cartProduct = cart.FirstOrDefault(p => p.Id == productId);
            if (cartProduct == null)
            {
                return NotFound("Produit non trouvé dans le panier.");
            }

            await _cartRepository.RemoveProductFromCartAsync(userEmail, productId);
            return Ok("Produit supprimé du panier.");
        }

        // DELETE
        // Vider le panier
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userEmail = User.Identity.Name; // Récupère l'email de l'utilisateur connecté
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Vous devez être connecté pour vider votre panier. Veuillez vous connecter !");
            }

            await _cartRepository.ClearCartAsync(userEmail);
            return Ok("Le Panier a bien été vidé !");
        }
    }
}