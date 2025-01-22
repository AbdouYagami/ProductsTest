using ProductApi.Models;
using System.Xml;
using Newtonsoft.Json;

namespace ProductApi.Repositories
{
    // Dans les consignes, on nous demande de gérer les requètes en JSON ou SQL, j'ai choisis de le faire en JSON
    // Par contre, j'ai choisis d'utiliser des Repository pour que ce soit + flexible et + simple si on souhaite gérer les données en SQL aussi
    public class JsonProductRepository
    {
        private const string FilePath = "products.json"; // Le fichier JSON pour stocker les produits et qui servira de fichier "test" pour la suite

        // Récupère tous les produits
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await ReadFromFileAsync();
        }

        // Récupérer un produit par ID
        public async Task<Product> GetProductByIdAsync(int id)
        {
            var products = await ReadFromFileAsync();
            var product = products.FirstOrDefault(p => p.Id == id);
            return product;
        }

        // Ajouter un produit
        public async Task AddProductAsync(Product product)
        {
            var products = await ReadFromFileAsync();
            products.Add(product);
            await WriteToFileAsync(products);
        }

        // Mettre à jour un produit
        // J'aurais voulu utiliser l'auto mappeur comme je m'étais habituer à le faire chez le Groupe Atlantic mais je ne voulais pas encombrer le projet
        public async Task UpdateProductAsync(Product updatedProduct)
        {
            var products = await ReadFromFileAsync();
            var existingProduct = products.FirstOrDefault(p => p.Id == updatedProduct.Id);

            if (existingProduct != null)
            {
                // Liste des propriétés à mettre à jour
                var propertiesToUpdate = new Dictionary<string, Action<Product, Product>>()
                {
                    { "Code", (oldProduct, newProduct) => oldProduct.Code = string.IsNullOrEmpty(newProduct.Code) ? oldProduct.Code : newProduct.Code },
                    { "Name", (oldProduct, newProduct) => oldProduct.Name = string.IsNullOrEmpty(newProduct.Name) ? oldProduct.Name : newProduct.Name },
                    { "Description", (oldProduct, newProduct) => oldProduct.Description = string.IsNullOrEmpty(newProduct.Description) ? oldProduct.Description : newProduct.Description },
                    { "Image", (oldProduct, newProduct) => oldProduct.Image = string.IsNullOrEmpty(newProduct.Image) ? oldProduct.Image : newProduct.Image },
                    { "Category", (oldProduct, newProduct) => oldProduct.Category = string.IsNullOrEmpty(newProduct.Category) ? oldProduct.Category : newProduct.Category },
                    { "Price", (oldProduct, newProduct) => oldProduct.Price = newProduct.Price > 0 ? newProduct.Price : oldProduct.Price },
                    { "Quantity", (oldProduct, newProduct) => oldProduct.Quantity = newProduct.Quantity > 0 ? newProduct.Quantity : oldProduct.Quantity },
                    { "InternalReference", (oldProduct, newProduct) => oldProduct.InternalReference = string.IsNullOrEmpty(newProduct.InternalReference) ? oldProduct.InternalReference : newProduct.InternalReference },
                    { "ShellId", (oldProduct, newProduct) => oldProduct.ShellId = newProduct.ShellId > 0 ? newProduct.ShellId : oldProduct.ShellId },
                    { "InventoryStatus", (oldProduct, newProduct) => oldProduct.InventoryStatus = newProduct.InventoryStatus != 0 ? newProduct.InventoryStatus : oldProduct.InventoryStatus },
                    { "Rating", (oldProduct, newProduct) => oldProduct.Rating = newProduct.Rating >= 0 ? newProduct.Rating : oldProduct.Rating },
                    { "CreatedAt", (oldProduct, newProduct) => oldProduct.CreatedAt = newProduct.CreatedAt != default ? newProduct.CreatedAt : oldProduct.CreatedAt },
                    { "UpdatedAt", (oldProduct, newProduct) => oldProduct.UpdatedAt = newProduct.UpdatedAt != default ? newProduct.UpdatedAt : oldProduct.UpdatedAt }
                };

                // Mise à jour des propriétés
                foreach (var property in propertiesToUpdate)
                {
                    property.Value(existingProduct, updatedProduct);
                }

                await WriteToFileAsync(products);
            }
        }
        // Supprimer un produit
        public async Task DeleteProductAsync(int id)
        {
            var products = await ReadFromFileAsync();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                await WriteToFileAsync(products);
            }
        }

        // recup des données du fichier json
        private async Task<List<Product>> ReadFromFileAsync()
        {
            if (!File.Exists(FilePath))
                return new List<Product>(); // Retourne une liste vide si le fichier n'existe pas

            var json = await File.ReadAllTextAsync(FilePath);
            return JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
        }

        // ajout des données dans le fichier json
        private async Task WriteToFileAsync(List<Product> products)
        {
            var json = JsonConvert.SerializeObject(products, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(FilePath, json);
        }
    }
}
