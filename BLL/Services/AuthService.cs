using BLL.DTO;
using DAL.EF;
using DAL.Entities;

namespace BLL.Services
{
    public class AuthService : IDisposable
    {
        private readonly IUnitOfWork _db;

        public AuthService()
        {
            _db = new UnitOfWork();
        }

        public UserDTO Register(string nick, string email, string pass, string confirm)
        {
            if (pass != confirm) 
                throw new Exception("Паролі не збігаються!");

            if (_db.Users.GetAll().Any(u => u.Email == email))
                throw new Exception("Користувач з таким Email вже існує!");

            var user = new User 
            { 
                Nickname = nick, 
                Email = email, 
                Password = pass 
            };

            _db.Users.Create(user);
            _db.Save();

            return MapToDTO(user);
        }

        public UserDTO? Login(string email, string pass)
        {
            User user = null;
            foreach (var u in _db.Users.GetAll())
            {
                if (u.Email == email && u.Password == pass)
                {
                    user = u;
                    break;
                }
            }

            if (user == null)
            {
                return null;
            }
            else
            {
                return MapToDTO(user);
            }
        }

        private UserDTO MapToDTO(User user)
        {
            return new UserDTO 
            {
                Id = user.Id, 
                Nickname = user.Nickname, 
                Email = user.Email 
            };
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}