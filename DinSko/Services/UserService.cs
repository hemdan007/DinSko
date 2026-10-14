using DinSko.Repositories;
using DinSko.Models;
namespace DinSko.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public User GetById(int id)
        {
            return _userRepository.GetById(id);
        }
        public User GetByEmail(string email)
        {
            return _userRepository.GetByEmail(email);
        }

        public void Add(User user)
        {
            _userRepository.Add(user);
        }

        public void Update(User user)
        {
            _userRepository.Update(user);
        }

        public void Delete(int id)
        {
            _userRepository.Delete(id);
        }
        public void ChangePassword(int userId, string passwordHash)
        {
            _userRepository.ChangePassword(userId, passwordHash);
        }
    }
}