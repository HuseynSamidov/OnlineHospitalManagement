using Application.DTOs.UserDTOs;
using Application.Shared;
using Domain.Entities;

namespace Application.Abstracts.Services;

    //Task<BaseResponse<AppUser>> RegisterAsync(RegisterDto model);
    //Task<BaseResponse<TokenResponse>> LoginAsync(LoginDto dto);
    ////Task<BaseResponse<LoginResponseDto>> RefreshTokenLoginAsync(string refreshToken);
    //Task<BaseResponse<bool>> LogoutAsync(string userId);
    //Task<BaseResponse<AppUser>> GetUserByIdAsync(string userId);


    public interface IUserService
    {
        Task<BaseResponse<TokenResponse>> RegisterAsync(RegisterDto dto);
        Task<BaseResponse<TokenResponse>> LoginAsync(LoginDto dto);
        Task<BaseResponse<string>> UpdateUserAsync(UpdateDto dto);
        Task<BaseResponse<TokenResponse>> ResetPasswordAsync(UserResetPasswordDto dto);
        Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string token);
        Task<BaseResponse<List<UserProfileDto>>> GetAllAsync();
        Task<BaseResponse<UserProfileDto>> GetByIdAsync(string id);
        Task<BaseResponse<string>> AddRoleAsync(UserAddRoleDto dto);
        Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequestDto request);
    }

