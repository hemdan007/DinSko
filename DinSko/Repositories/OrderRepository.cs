using DinSko.Models;
using Microsoft.Data.SqlClient;
namespace DinSko.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Order> GetAll()
        {
            return null;
        }

        public Order GetById(int id)
        {
            return null;
        }

        public IEnumerable<Order> GetByUserId(int userId)
        {
            return null;
        }

        public void Create(Order order)
        {
        }

        public void UpdateStatus(int orderId, OrderStatus status)
        {
        }

        public void Delete(int orderId)
        {
        }

        public IEnumerable<OrderItem> GetItems(int orderId)
        {
            return null;
        }

        public void AddItem(OrderItem item)
        {
        }

        public void DeleteItem(int itemId)
        {
        }
    }
}