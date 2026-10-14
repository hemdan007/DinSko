using DinSko.Models;
namespace DinSko.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetById(int id);
        User GetByEmail(string email);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
        void ChangePassword(int userId, string passwordHash);
    }
}