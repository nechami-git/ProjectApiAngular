using AutoMapper;
using server.BLL.Intarfaces;
using server.DAL.Interfaces;
using server.Exceptions;
using server.Models;
using server.Models.DTO;


namespace server.BLL
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserDal _userDal;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public UserBLL(IUserDal userDal, IMapper mapper, IConfiguration configuration)
        {
            _userDal = userDal;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            var users = await _userDal.GetUsers();
            return _mapper.Map<List<UserDTO>>(users);
        }
        public async Task<UserDTO> GetById(int id)
        {
            var user = await _userDal.GetById(id);
            if (user == null)
            {
                throw new NotFoundException($"User {id} not found");
            }
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<AuthUserDTO> Register(UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Email) || string.IsNullOrWhiteSpace(userDTO.FirstName)
                || string.IsNullOrWhiteSpace(userDTO.Phone) || string.IsNullOrWhiteSpace(userDTO.Password)
                 || string.IsNullOrWhiteSpace(userDTO.LastName))
                throw new ArgumentException("כל השדות (שם, שם משפחה, אימייל, סיסמה, טלפון) הם חובה.");

            if (userDTO.Password.Length < 8)
            {
                throw new ArgumentException("הסיסמה חייבת להכיל לפחות 8 תווים.");
            }
                if (!System.Text.RegularExpressions.Regex.IsMatch(userDTO.Phone, @"^\d{9,10}$"))
                {
                    throw new ArgumentException("מספר הטלפון אינו תקין. יש להזין ספרות בלבד.");
                }

                if (!userDTO.Email.Contains("@") || !userDTO.Email.Contains("."))
                {
                    throw new ArgumentException("כתובת האימייל אינה תקינה.");
                }
                var existingUser = await _userDal.GetByEmail(userDTO.Email!);
                if (existingUser != null)
                {
                    throw new ConflictException($"משתמש עם האימייל {userDTO.Email} כבר קיים במערכת.");
                }

            // 6. יצירת המשתמש והצפנת הסיסמה
            var user = _mapper.Map<UserModel>(userDTO);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password!);
            user.Role = Role.Manager; 

            // שליחה לדאל - חשוב שהדאל יחזיר את האובייקט כדי שנקבל את ה-ID שנוצר ב-DB
            var savedUser = await _userDal.Register(user);

            // החזרת הנתונים שדרושים לטוקן
            return new AuthUserDTO
            {
                Id = savedUser.Id,
                Email = savedUser.Email,
                FirstName = savedUser.FirstName,
                Role = savedUser.Role
            };
        }

        public async Task<AuthUserDTO?> Login(string? email, string? password)
        {

            var user = await _userDal.GetByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }
            return _mapper.Map<AuthUserDTO>(user);
        }




    }
}