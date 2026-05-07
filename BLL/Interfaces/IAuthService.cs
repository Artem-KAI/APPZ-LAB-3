using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IAuthService : IDisposable
    {
        UserDTO Register(string nick, string email, string pass, string confirm);
        UserDTO? Login(string email, string pass);
    }
}
