using DinSko.Models;
using DinSko.Repositories;
namespace DinSko.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public IEnumerable<Product> GetAll() { return _productRepository.GetAll(); }

        public Product GetById(int id) { return _productRepository.GetById(id); }

        public void Add(Product product) { _productRepository.Add(product); }

        public void Update(Product product) { _productRepository.Update(product); }

        public void Delete(int id) { _productRepository.Delete(id); }
    }
}