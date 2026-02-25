using server.Models;
using System.Drawing;

namespace server.DAL.Interfaces
{
    public interface IDonorDAL
    {
        Task Post(DonorModel donor);
        Task<DonorModel?> GetById(int id);
        Task<DonorModel?> GetByEmail(string email);
        Task Delete(int id);
        Task<DonorModel> Put(DonorModel donor);
        Task<List<DonorModel>> GetDonors(string? name = null, string? email = null, string? giftName = null);
        Task<DonorModel?> GetByIdentity(string identityNumber);
    }
}
