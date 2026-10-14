using DinSko.Models;
using DinSko.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DinSko.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ProductService _productService;
        private readonly ProductVariantService _productVariantService;

        public IndexModel(
            ILogger<IndexModel> logger,
            ProductService productService,
            ProductVariantService productVariantService)
        {
            _logger = logger;
            _productService = productService;
            _productVariantService = productVariantService;
        }

        public IEnumerable<Product> Products { get; set; } =
            new List<Product>();

        public List<ProductWithVariants> ProductList { get; set; } =
            new List<ProductWithVariants>();

        public class ProductWithVariants
        {
            public Product Product { get; set; }
            public IEnumerable<ProductVariant> Variants { get; set; }
        }

        public void OnGet()
        {
            Products = _productService.GetAll();

            // Get the available sizes for each product
            foreach (Product product in Products)
            {
                ProductList.Add(new ProductWithVariants
                {
                    Product = product,
                    Variants = _productVariantService.GetByProductId(product.ProductId)
                        .Where(variant => variant.Stock > 0)
                        .ToList()
                });
            }
        }
    }
}