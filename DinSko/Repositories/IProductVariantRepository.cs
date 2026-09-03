using DinSko.Models;
namespace DinSko.Repositories
{
    public interface IProductVariantRepository
    {
        IEnumerable<ProductVariant> GetAll();
        ProductVariant GetById(int id);
        IEnumerable<ProductVariant> GetByProductId(int productId);
        void Add(ProductVariant productVariant);
        void Update(ProductVariant productVariant);
        void Delete(int id);
    }
}
