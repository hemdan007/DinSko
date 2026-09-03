using DinSko.Models;
using DinSko.Repositories;
namespace DinSko.Services
{
    public class ProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;
        public ProductVariantService(IProductVariantRepository productVariantRepository)
        {
            _productVariantRepository = productVariantRepository;
        }

        public IEnumerable<ProductVariant> GetAll()
        {
            return _productVariantRepository.GetAll();
        }
        public ProductVariant GetById(int id)
        {
            return _productVariantRepository.GetById(id);
        }
        public IEnumerable<ProductVariant> GetByProductId(int productId)
        {
            return _productVariantRepository.GetByProductId(productId);
        }
        public void Add(ProductVariant productVariant)
        {
            _productVariantRepository.Add(productVariant);
        }
        public void Update(ProductVariant productVariant) 
        { 
            _productVariantRepository.Update(productVariant);
        }
        public void Delete(int id)
        {
            _productVariantRepository.Delete(id);
        }
    }
}
