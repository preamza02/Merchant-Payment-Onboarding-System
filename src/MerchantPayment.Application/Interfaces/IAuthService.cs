using MerchantPayment.Application.DTOs;

namespace MerchantPayment.Application.Interfaces;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
