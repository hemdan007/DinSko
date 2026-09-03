using DinSko.Models;
using Microsoft.Data.SqlClient;
namespace DinSko.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly string _connectionString;

        public CartRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public Cart GetByUserId(int userId)
        {
            return null;
        }

        public void Create(Cart cart)
        {
        }

        public void Delete(int cartId)
        {
        }

        public IEnumerable<CartItem> GetItems(int cartId)
        {
            return null;
        }

        public void AddItem(CartItem item)
        {
        }

        public void UpdateItem(CartItem item)
        {
        }

        public void DeleteItem(int itemId)
        {
        }

        public void ClearCart(int cartId)
        {
        }
    }

}
