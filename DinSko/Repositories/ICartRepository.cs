using DinSko.Models;

namespace DinSko.Repositories
{
    public interface ICartRepository
    {
        Cart GetByUserId(int userId);
        void Create(Cart cart);
        void Delete(int cartId);
        IEnumerable<CartItem> GetItems(int cartId);
        void AddItem(CartItem item);
        void UpdateItem(CartItem item);
        void DeleteItem(int itemId);
        void ClearCart(int cartId);
    }
}
