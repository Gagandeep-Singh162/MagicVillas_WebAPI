using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private string _secretKey;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _roleManager = roleManager;
        }

        public bool isUniqueUser(string userName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == userName);
            return user == null;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            bool isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || !isValidPassword)
            {
                return new()
                {
                    Token = "",
                    User = null
                };
            }
            else
            {
                //if user was found generate JWT token
                var roles = await _userManager.GetRolesAsync(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName.ToString()),
                        new Claim(ClaimTypes.Role, roles.First())
                    }),
                    Expires = DateTime.UtcNow.AddHours(10),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                LoginResponseDto loginResponseDto = new()
                {
                    Token = tokenHandler.WriteToken(token),
                    User = _mapper.Map<UserDto>(user),
                };

                return loginResponseDto;
            }
        }

        public async Task<UserDto> Register(RegisterationRequestDto registerationRequestDto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registerationRequestDto.UserName,
                Email = registerationRequestDto.UserName,
                NormalizedEmail = registerationRequestDto.UserName.ToUpper(),
                Name = registerationRequestDto.Name,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDto.Password);
                if (result.Succeeded) 
                {

                    //Not a good approach.. a simple hack right now
                    if (! await _roleManager.RoleExistsAsync("admin")){
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                    }
                    await _userManager.AddToRoleAsync(user, "admin");
                    var userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registerationRequestDto.UserName);
                    return _mapper.Map<UserDto>(userToReturn);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return new UserDto();
        }
    }
}
