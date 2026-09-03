using DinSko.Models;
using DinSko.Repositories;
namespace DinSko.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IEnumerable<Order> GetAll()
        {
            return _orderRepository.GetAll();
        }

        public Order GetById(int id)
        {
            return _orderRepository.GetById(id);
        }

        public IEnumerable<Order> GetByUserId(int userId)
        {
            return _orderRepository.GetByUserId(userId);
        }

        public void Create(Order order)
        {
            _orderRepository.Create(order);
        }

        public void UpdateStatus(int orderId, OrderStatus status)
        {
            _orderRepository.UpdateStatus(orderId, status);
        }

        public void Delete(int orderId)
        {
            _orderRepository.Delete(orderId);
        }

        public IEnumerable<OrderItem> GetItems(int orderId)
        {
            return _orderRepository.GetItems(orderId);
        }

        public void AddItem(OrderItem item)
        {
            _orderRepository.AddItem(item);
        }

        public void DeleteItem(int itemId)
        {
            _orderRepository.DeleteItem(itemId);
        }
    }
}