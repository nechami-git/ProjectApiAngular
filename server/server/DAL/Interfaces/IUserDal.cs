using server.Models;
using System.Collections.Generic;
using System.Threading.Tasks; 

namespace server.DAL.Interfaces
{
    public interface IUserDal
    {

        Task<UserModel> Register(UserModel user);
        Task<UserModel?> GetById(int id);
        Task<List<UserModel>> GetUsers();
        Task<UserModel?> GetByEmail(string? email);
    }
}