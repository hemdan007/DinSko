using DinSko.Models;
using Microsoft.Data.SqlClient;
namespace DinSko.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly string _connectionString;

        public ProductVariantRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IEnumerable<ProductVariant> GetAll()
        {
            return null;
        }
        public ProductVariant GetById(int id)
        {
            return null;
        }
        public IEnumerable<ProductVariant> GetByProductId(int productId)
        {
            return null;
        }
        public void Add(ProductVariant productVariant)
        {
        }
        public void Update(ProductVariant productVariant)
        {
        }
        public void Delete(int id)
        {
        }
    }
}
