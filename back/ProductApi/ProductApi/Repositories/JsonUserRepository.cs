using System.Text.Json;
using ProductApi.Models;

namespace ProductApi.Repositories
{
    // Comme pour les produits j'ai choisis de gérer les requètes en JSON mais j'ai utilisé les Repository pour que ce soit + flexible/simple dans le cas d'ajout de la gestion pour données SQL
    public class JsonUserRepository
    {
        
        private readonly string _filePath = "users.json"; // fichier test pour les données en JSON des utilisateurs

        public JsonUserRepository()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]"); // Crée un fichier JSON vide s'il n'existe pas
            }
        }
        // Récupération de tous les utilisateurs à partir du fichier JSON, si le fichier est vide on crée une liste vide
        public async Task<List<User>> GetAllUsersAsync()
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        // Retrouver un utilisateur à partir de son mail
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var users = await GetAllUsersAsync();
            return users.FirstOrDefault(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            // J'avais le choix sois entre créé l'admin en "dur" ou qu'on le fasse directement sur le swagger/postman via la requète
            // J'ai choisis la deuxième option car mettre un mdp dans le code c'est vraiment pas propre et pas fou niveau sécurité
            // Donc au début les users seront vide et ça va s'ajouter petit à petit.
            var users = await GetAllUsersAsync();
            users.Add(user);

            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

    }
}
