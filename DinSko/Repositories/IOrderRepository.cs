using DinSko.Models;
namespace DinSko.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll();
        Order GetById(int id);
        IEnumerable<Order> GetByUserId(int userId);
        void Create(Order order);
        void UpdateStatus(int orderId, OrderStatus status);
        void Delete(int orderId);
        IEnumerable<OrderItem> GetItems(int orderId);
        void AddItem(OrderItem item);
        void DeleteItem(int itemId);
    }
}