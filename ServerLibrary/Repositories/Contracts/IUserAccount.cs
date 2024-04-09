using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(RegisterDto user);
        Task<LoginResponse> SignInAsync(LoginDto user);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token);
    }
}
