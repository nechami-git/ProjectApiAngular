using server.DAL.Interfaces;
using server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace server.DAL
{
    public class UserDal : IUserDal
    {
        private readonly ChineseSaleContext _context;

        public UserDal(ChineseSaleContext chineseSaleContext)
        {
            _context = chineseSaleContext;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            return await _context.Users.Include(t => t.Tickets).AsNoTracking().ToListAsync(); 
        }

        public async Task<UserModel?> GetById(int id)
            {
            return await _context.Users
                .Include(t => t.Tickets)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
             }

        public async Task<UserModel?> GetByEmail(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserModel> Register(UserModel user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user; 
        }

    }
}