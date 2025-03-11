using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool isUniqueUser(string userName);

        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

        Task<LocalUser> Register(RegisterationRequestDto registerationRequestDto);
    }
}
