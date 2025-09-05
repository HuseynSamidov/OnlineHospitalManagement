using Application.Abstracts.Services;
using Application.DTOs.UserDTOs;
using Application.Shared;
using Application.Shared.Settings;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistence.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Persistence.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JWTSettings _jwtSettings;
    private readonly AppDbContext _context;

    public UserService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
          AppDbContext context,
        IOptions<JWTSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
        _context = context;
    }

    public async Task<BaseResponse<TokenResponse>> RegisterAsync(RegisterDto dto)
    {
        // 1. Email artıq mövcuddurmu
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return new BaseResponse<TokenResponse>("This email is already registered", HttpStatusCode.BadRequest);

        // 2. Yeni AppUser yaradılır
        var newUser = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var createResult = await _userManager.CreateAsync(newUser, dto.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            return new BaseResponse<TokenResponse>($"User creation failed: {errors}", HttpStatusCode.BadRequest);
        }

        // 3. Role əlavə edilir (Patient/Doctor)
        var roleName = dto.Role;
        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new IdentityRole(roleName));

        await _userManager.AddToRoleAsync(newUser, roleName);

        // 4. Əlavə məlumatlar Patient/Doctor üçün
        if (roleName.Equals("Patient", StringComparison.OrdinalIgnoreCase))
        {
            var patient = new Patient
            {
                AppUserId = newUser.Id,
                DateOfBirth = dto.DateOfBirth!.Value,
                Gender = dto.Gender!,
                MissedTurns = 0
            };
            // DbContext istifadə edərək save
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }
        else if (roleName.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
        {
            if (!dto.DepartmentId.HasValue)
            {
                throw new ArgumentException("Department seçilməlidir");
            }

            var doctor = new Doctor
            {
                AppUserId = newUser.Id,
                Specialization = dto.Specialization!,
                DepartmentId = dto.DepartmentId.Value   
            };
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }

        // 5. Email təsdiqi linki (SMTP ilə sonradan göndəriləcək)
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        var emailConfirmLink = $"https://localhost:7046/api/Users/ConfirmEmail?userId={newUser.Id}&token={HttpUtility.UrlEncode(emailToken)}";

        Console.WriteLine("Confirm email link: " + emailConfirmLink);

        // 6. JWT token yaradılır
        var tokenResponse = await GenerateJwtToken(newUser);

        return new BaseResponse<TokenResponse>("User registered successfully", tokenResponse, HttpStatusCode.Created);
    }


    public Task<BaseResponse<TokenResponse>> LoginAsync(LoginDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string token)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<string>> UpdateUserAsync(UpdateDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<UserProfileDto>> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<List<UserProfileDto>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }


    public Task<BaseResponse<string>> AddRoleAsync(UserAddRoleDto dto)
    {
        throw new NotImplementedException();
    }


    public Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<TokenResponse>> ResetPasswordAsync(UserResetPasswordDto dto)
    {
        throw new NotImplementedException();
    }


    #region Tokens
    private async Task<TokenResponse> GenerateJwtToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email)
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var roleName in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is not null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                var rolePermissions = roleClaims
                    .Where(c => c.Type == "Permission")
                    .ToList();
                foreach (var permission in rolePermissions)
                {
                    claims.Add(new Claim("Permission", permission.Value));
                }
            }
        }
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_jwtSettings.ExpiryMinutes).AddMinutes(1);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            NotBefore = now,
            IssuedAt = now,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };


        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryDate = DateTime.UtcNow.AddHours(2);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryDate;
        await _userManager.UpdateAsync(user);

        return new TokenResponse
        {
            Token = jwt,
            ExpireDate = tokenDescriptor.Expires!.Value,
            RefreshToken = refreshToken,
        };

    }
    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
    #endregion


}
