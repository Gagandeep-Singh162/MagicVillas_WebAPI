using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private string _secretKey;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool isUniqueUser(string userName)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.UserName == userName);
            return user == null;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower() && u.Password == loginRequestDto.Password);

            if (user == null)
            {
                return null;
            }
            else
            {
                //if user was found generate JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddHours(10),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                LoginResponseDto loginResponseDto = new()
                {
                    Token = tokenHandler.WriteToken(token),
                    User = user
                };

                return loginResponseDto;
            }
        }

        public async Task<LocalUser> Register(RegisterationRequestDto registerationRequestDto)
        {
            LocalUser user = new LocalUser()
            {
                UserName = registerationRequestDto.UserName,
                Password = registerationRequestDto.Password,
                Name = registerationRequestDto.Name,
                Role = registerationRequestDto.Role,
            };

            await _db.LocalUsers.AddAsync(user);
            await _db.SaveChangesAsync();

            user.Password = "";
            return user;
        }
    }
}
