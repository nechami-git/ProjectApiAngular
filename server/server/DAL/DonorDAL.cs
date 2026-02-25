using Microsoft.EntityFrameworkCore;
using server.DAL.Interfaces;
using server.Models;
using System.Drawing;

namespace server.DAL
{
    public class DonorDAL : IDonorDAL
    {
        private readonly ChineseSaleContext _chSaContext;
        public DonorDAL(ChineseSaleContext chSaContext)
        {
            _chSaContext = chSaContext;
        }
        public async Task<List<DonorModel>> GetDonors(string? name, string? email, string? giftName)
        {
            IQueryable<DonorModel> query = _chSaContext.Donors.Include(d => d.Donations);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(d => d.FirstName!.Contains(name) || d.LastName!.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(d => d.Email!.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(giftName))
            {
                query = query.Where(d => d.Donations.Any(g => g.Name!.Contains(giftName)));
            }

            return await query.AsNoTracking().ToListAsync();
        }

       
        public async Task Post(DonorModel donor)
        {
             _chSaContext.Donors.Add(donor);
            await _chSaContext.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var donor = await _chSaContext.Donors.FindAsync(id);

            if (donor != null)
            {
                _chSaContext.Donors.Remove(donor);
                await _chSaContext.SaveChangesAsync();
            }
        }

        public async Task<DonorModel> Put(DonorModel donor)
        {
            var existingDonor = await _chSaContext.Donors.FindAsync(donor.Id);
            if (existingDonor == null)
            {
                return null!;
            }
            _chSaContext.Entry(existingDonor).CurrentValues.SetValues(donor);
            await _chSaContext.SaveChangesAsync();
            return existingDonor;
        }
        public async Task<DonorModel?> GetById(int id)
        {
            return await _chSaContext.Donors
                .Include(d => d.Donations) 
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<DonorModel?> GetByEmail(string email)
        {
            return await _chSaContext.Donors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Email == email);
        }
        public async Task<DonorModel?> GetByIdentity(string identityNumber)
        {
            return await _chSaContext.Donors.FirstOrDefaultAsync(d => d.IdentityNumber == identityNumber);
        }

        
    }
}
