using server.Models;
using server.Models.DTO;

namespace server.BLL.Intarfaces
{
    public interface ITokenBLL
    {
        string GenerateToken(AuthUserDTO user);
    }
}
