using DinSko.Repositories;
using DinSko.Models;

namespace DinSko.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public Cart GetByUserId(int userId)
        {
            return _cartRepository.GetByUserId(userId);
        }

        public void Create(Cart cart)
        {
            _cartRepository.Create(cart);
        }

        public void Delete(int cartId)
        {
            _cartRepository.Delete(cartId);
        }

        public IEnumerable<CartItem> GetItems(int cartId)
        {
            return _cartRepository.GetItems(cartId);
        }

        public void AddItem(CartItem item)
        {
            _cartRepository.AddItem(item);
        }

        public void UpdateItem(CartItem item)
        {
            _cartRepository.UpdateItem(item);
        }

        public void DeleteItem(int itemId)
        {
            _cartRepository.DeleteItem(itemId);
        }

        public void ClearCart(int cartId)
        {
            _cartRepository.ClearCart(cartId);
        }
    }
}
