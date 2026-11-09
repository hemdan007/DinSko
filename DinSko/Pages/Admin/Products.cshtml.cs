
using DinSko.Models;
using DinSko.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DinSko.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ProductsModel : PageModel
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductsModel> _logger;

        public ProductsModel(
            ProductService productService,
            ILogger<ProductsModel> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public IEnumerable<Product> Products { get; set; } =
            new List<Product>();

        [BindProperty]
        public Product NewProduct { get; set; } = new Product();

        [BindProperty]
        public Product EditProduct { get; set; } = new Product();

        public int? EditingProductId { get; set; }

        public void OnGet()
        {
            LoadProducts();
        }

        // load all products from the database
        private void LoadProducts()
        {
            Products = _productService.GetAll();
        }

        // create a new product

        // Create a new product
        public IActionResult OnPostCreate()
        {
            // Clear validation results from the other form
            ModelState.Clear();

            // Validate only the new product
            if (!TryValidateModel(NewProduct, nameof(NewProduct)))
            {
                _logger.LogWarning(
                    "Admin attempted to create a product with invalid data.");

                LoadProducts();
                return Page();
            }

            _productService.Add(NewProduct);

            // Log successful product creation
            _logger.LogInformation(
                "Admin created product: {ProductName}.",
                NewProduct.Name);

            return RedirectToPage();
        }

        // load the selected product for editing
        public IActionResult OnPostEdit(int id)
        {
            Product product = _productService.GetById(id);

            if (product == null)
            {
                _logger.LogWarning(
                    "Admin attempted to edit a non-existing product with ID {ProductId}.",
                    id);

                return NotFound();
            }

            EditProduct = product;
            EditingProductId = id;

            LoadProducts();

            return Page();
        }

        // update an existing product
        public IActionResult OnPostUpdate()
        {
            // clear validation results from the other form
            ModelState.Clear();

            // validate only the product being edited
            if (!TryValidateModel(EditProduct, nameof(EditProduct)))
            {
                _logger.LogWarning(
                    "Admin attempted to update product {ProductId} with invalid data.",
                    EditProduct.ProductId);

                EditingProductId = EditProduct.ProductId;
                LoadProducts();
                return Page();
            }

            Product existingProduct =
                _productService.GetById(EditProduct.ProductId);

            if (existingProduct == null)
            {
                _logger.LogWarning(
                    "Admin attempted to update a non-existing product with ID {ProductId}.",
                    EditProduct.ProductId);

                return NotFound();
            }

            _productService.Update(EditProduct);

            // log successful product update
            _logger.LogInformation(
                "Admin updated product with ID {ProductId}.",
                EditProduct.ProductId);

            return RedirectToPage();
        }

        // delete a product
        public IActionResult OnPostDelete(int id)
        {
            Product product = _productService.GetById(id);

            if (product == null)
            {
                _logger.LogWarning(
                    "Admin attempted to delete a non-existing product with ID {ProductId}.",
                    id);

                return NotFound();
            }

            _productService.Delete(id);

            // log successful product deletion
            _logger.LogInformation(
                "Admin deleted product with ID {ProductId}.",
                id);

            return RedirectToPage();
        }
    }
}