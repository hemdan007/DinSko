using System.ComponentModel.DataAnnotations;

namespace DinSko.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        // Product name is required and cannot exceed 100 characters
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // Description is optional and cannot exceed 1000 characters
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        // Product price must be greater than zero
        // Product price must be greater than zero
        [Range(0.01, 999999.99,
        ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }
    }
}