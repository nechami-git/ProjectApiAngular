using AutoMapper;
using server.BLL.Intarfaces;
using server.DAL.Interfaces;
using server.Exceptions;
using server.Models;
using server.Models.DTO;

namespace server.BLL
{
    public class DonorBLL : IDonorBLL
    {
        private readonly IDonorDAL _donorDAL;
        private readonly IMapper _mapper;

        public DonorBLL(IDonorDAL donorDAL, IMapper mapper)
        {
            _donorDAL = donorDAL;
            _mapper = mapper;
        }

        public async Task<List<DonorDTO>> GetDonors(string? name, string? email, string? giftName)
        {
            var donors = await _donorDAL.GetDonors(name, email, giftName);
            return _mapper.Map<List<DonorDTO>>(donors);
        }

        public async Task<DonorDTO?> GetById(int id)
        {
            var donor = await _donorDAL.GetById(id);
            if (donor == null)
            {
                throw new NotFoundException($"תורם עם מזהה {id} לא נמצא במערכת.");
            }
            return _mapper.Map<DonorDTO>(donor);
        }

        public async Task Post(DonorDTO donorDto)
        {
            if (donorDto == null)
                throw new ArgumentNullException(nameof(donorDto));

            if (string.IsNullOrWhiteSpace(donorDto.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(donorDto.IdentityNumber))
                throw new ArgumentException("IdentityNumber is required.");

            var existDonor = await _donorDAL.GetByIdentity(donorDto.IdentityNumber);

            var existingEmail = await _donorDAL.GetByEmail(donorDto.Email);
            if (existingEmail != null)
            {
                throw new ConflictException("תורם עם כתובת אימייל זו כבר קיים במערכת.");
            }

            if (existDonor != null)
            {
                throw new ConflictException("תורם עם כתובת ת.ז זו כבר קיים במערכת.");
            }


            var donor = _mapper.Map<DonorModel>(donorDto);
            await _donorDAL.Post(donor);
        }

        public async Task<DonorDTO> Put(int id , DonorDTO donorDto)
        {
            if (donorDto == null)
                throw new ArgumentNullException();

            var existingDonor = await _donorDAL.GetById(id);
            if (existingDonor == null)
                throw new NotFoundException("לא ניתן לעדכן תורם שלא קיים.");

            var donorToUpdate = _mapper.Map<DonorModel>(donorDto);
            donorToUpdate.Id = id;


            var updated = await _donorDAL.Put(donorToUpdate);
            return _mapper.Map<DonorDTO>(updated);
        }

        public async Task Delete(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID");

            var existingDonor = await _donorDAL.GetById(id);
            if (existingDonor == null)
            {
                throw new KeyNotFoundException($"Cannot delete. Donor with ID {id} not found.");
            }

            await _donorDAL.Delete(id);
        }
        public async Task<DonorDTO?> GetDonorById(int id)
        {
            var donor = await _donorDAL.GetById(id);
            if (donor == null)
            {
                throw new NotFoundException($"Donor {id} not found");
            }
            return _mapper.Map<DonorDTO>(donor);
        }

    }
}