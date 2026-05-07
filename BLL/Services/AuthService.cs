using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using System;
using System.Linq;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _db;

        public AuthService(IUnitOfWork db)
        {
            _db = db;
        }

        public UserDTO Register(string nick, string email, string pass, string confirm)
        { 
            if (pass != confirm)
            {
                throw new ArgumentException("Паролі не збігаються!");
            }
             
            var allUsers = _db.Users.GetAll();
            bool userAlreadyExists = allUsers.Any(u => u.Email == email);

            if (userAlreadyExists)
            {
                throw new InvalidOperationException("Користувач з таким Email вже існує!");
            }
             
            var newUser = new User
            {
                Nickname = nick,
                Email = email,
                Password = pass
            };
             
            _db.Users.Create(newUser);
            _db.Save();
             
            var userDto = MapToDTO(newUser);
            return userDto;
        }

        public UserDTO? Login(string email, string pass)
        { 
            var allUsers = _db.Users.GetAll();
             
            var user = allUsers.FirstOrDefault(u => u.Email == email && u.Password == pass);
             
            if (user != null)
            { 
                var userDto = MapToDTO(user);
                return userDto;
            }
            else
            { 
                return null;
            }
        }
         
        private static UserDTO MapToDTO(User user)
        {
            var dto = new UserDTO
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Email = user.Email
            };

            return dto;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}