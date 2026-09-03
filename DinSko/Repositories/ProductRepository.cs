using DinSko.Models;
using Microsoft.Data.SqlClient;
namespace DinSko.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Product> GetAll()
        {
            return null;
        }

        public Product GetById(int id)
        {
            return null;
        }

        public void Add(Product product)
        {
        }

        public void Update(Product product)
        {
        }

        public void Delete(int id)
        {
        }
    }
}