using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductApi.Models;
using ProductApi.Repositories;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly JsonProductRepository _productRepository;

    public ProductController()
    {
        _productRepository = new JsonProductRepository(); // Initialisation avec le dépôt JSON
    }

    // On va utilisé cette méthode pour imposer que l'utilisateur doit etre connecté pour faire appels à cet api sur la gestion des produits
    private bool IsTokenValid(string email)
    {
        // Charger les utilisateurs depuis le fichier JSON
        var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText("users.json"));

        // Vérifier si l'email existe
        return users.Any(u => u.Email == email);
    }

    // Vu les consignes forcément l'admin aura cet email donc on le garde ici pour l'utiliser sur les requète demander : ajout, update et suppression
    private bool IsAdmin(string email) => email == "admin@admin.com";

    // POST /products
    // Créer un nouveau produit 
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> AddProduct(Product product)
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // On vérifie si le token est bon
        if (string.IsNullOrEmpty(emailClaim) || !IsTokenValid(emailClaim))
        {
            return Unauthorized("Token invalide ou utilisateur non trouvé.");
        }

        // Vérification si l'utilisateur est admin
        if (!IsAdmin(emailClaim))
        {
            return Unauthorized("Seul l'administrateur peut ajouter des produits.");
        }

        // J'ai gérer quelques cas d'erreurs comme ID déjà utilisé (donc produit existant)
        var existingProduct = await _productRepository.GetProductByIdAsync(product.Id);
        if (existingProduct != null)
        {
            return BadRequest("Un produit avec le même ID existe déjà.");
        }

        await _productRepository.AddProductAsync(product);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    // GET /products
    // Récupérer tous les produits
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Vérifier si le token est valide
        if (string.IsNullOrEmpty(emailClaim) || !IsTokenValid(emailClaim))
        {
            return Unauthorized("Token invalide ou utilisateur non trouvé.");
        }

        var products = await _productRepository.GetAllProductsAsync();
        return Ok(products);
    }

    // GET /products/{id}
    // Récupérer les détails d'un produit par son ID
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Vérifier si le token est valide
        if (string.IsNullOrEmpty(emailClaim) || !IsTokenValid(emailClaim))
        {
            return Unauthorized("Token invalide ou utilisateur non trouvé.");
        }
        // Test si le produit existe
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = "Produit non trouvé." });
        }

        return Ok(product);
    }

    // PATCH /products/{id}
    // Mettre à jour les détails d'un produit
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<ActionResult> PatchProduct(int id, Product updatedProduct)
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Vérifier si le token est valide
        if (string.IsNullOrEmpty(emailClaim) || !IsTokenValid(emailClaim))
        {
            return Unauthorized("Token invalide ou utilisateur non trouvé.");
        }

        // Vérifier si l'utilisateur est admin
        if (!IsAdmin(emailClaim))
        {
            return Unauthorized("Seul l'administrateur peut modifier des produits.");
        }
        // Vu que l'id est présent aussi dans la requète, il faut que ce soit le MEME sur dans le champ du forumlaire du swagger par exemple
        if (updatedProduct.Id != id)
        {
            return BadRequest("L'ID du produit dans l'URL ne correspond pas à celui du produit dans la requête.");
        }

        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound("Le produit demandé n'existe pas.");
        }

        await _productRepository.UpdateProductAsync(updatedProduct);
        return NoContent(); // 204 - Mise à jour réussie
    }

    // DELETE /products/{id}
    // Supprimer un produit par son ID
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Vérifier si le token est valide
        if (string.IsNullOrEmpty(emailClaim) || !IsTokenValid(emailClaim))
        {
            return Unauthorized("Token invalide ou utilisateur non trouvé.");
        }

        // Vérifier si l'utilisateur est admin
        if (!IsAdmin(emailClaim))
        {
            return Unauthorized("Seul l'administrateur peut supprimer des produits.");
        }

        var product = await _productRepository.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound($"Produit avec l'ID {id} non trouvé.");
        }

        await _productRepository.DeleteProductAsync(id);
        return NoContent();
    }
}