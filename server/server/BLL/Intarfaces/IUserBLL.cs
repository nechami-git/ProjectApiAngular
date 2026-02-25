
using server.Models.DTO;

namespace server.BLL.Intarfaces
{
    public interface IUserBLL
    {
        Task<List<UserDTO>> GetUsers();
        Task<UserDTO> GetById(int id);
        Task<AuthUserDTO?> Login(string? email, string? password);
        Task<AuthUserDTO> Register(UserDTO userDTO);
    }
}
