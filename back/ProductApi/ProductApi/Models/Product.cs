namespace ProductApi.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    // J'ai choisis de mettre l'id en obligatoire vu que ça sera l'identifiant unique qui représentera le produit
    // Pour le reste j'aurais pu mettre que ce soit requis mais ayant pas des directives claires là dessus j'suis partis du principe qu'on peut créé un produit 
    // puis avec le PATCH on pourra modifier / mettre à jour les autres champs
   
    // Enum pour définir les statuts de stock
    public enum InventoryStatus
    {
        INSTOCK = 0,    // En stock

        LOWSTOCK = 1,   // Stock faible

        OUTOFSTOCK = 2  // Hors stock
    }

    public class Product
    {
        [Required(ErrorMessage = "L'ID du produit est obligatoire.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID doit être supérieur à 0.")]
        public int Id { get; set; }   
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        
        // J'aurais pu diversifier + les tests et message d'erreurs mais je me suis focaliser sur les erreurs importantes comme le prix ou la quantité qui doit etre positives

        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être positive.")]
        public int Quantity { get; set; }  
        public string InternalReference { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "L'ID de shell doit être positif.")]
        public int ShellId { get; set; }
        // Validation pour limiter les valeurs possibles de InventoryStatus à 0, 1, ou 2
        [Range(0, 2, ErrorMessage = "L'état d'inventaire doit être 0, 1 ou 2.")]
        public InventoryStatus InventoryStatus { get; set; } // Utilisation de l'Enum
        public decimal Rating { get; set; }  
        // J'ai utilisé des DateTime pour les dates, + pratique et + lisible lors des tests en postman ou swagger
        public DateTime CreatedAt { get; set; }  
        public DateTime UpdatedAt { get; set; }   
    }
}
