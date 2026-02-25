using server.Models;
using server.Models.DTO;
using System.Drawing;

namespace server.BLL.Intarfaces
{
    public interface IDonorBLL
    {
        Task Post(DonorDTO donorDto);    
        Task Delete(int id);   
        Task<DonorDTO> Put(int id, DonorDTO donorDto);
        Task<List<DonorDTO>> GetDonors(string? name = null, string? email = null, string? giftName = null);
        Task<DonorDTO?> GetById(int id);
    }
}

