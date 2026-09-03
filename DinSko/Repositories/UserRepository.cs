using DinSko.Models;
using DinSko.Repositories;
using Microsoft.Data.SqlClient;
namespace DinSko.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<User> GetAll()
        {
            return null;
        }

        public User GetById(int id)
        {
            return null;
        }

        public void Add(User user)
        {
        }

        public void Update(User user)
        {
        }

        public void Delete(int id)
        {
        }
    }
}
