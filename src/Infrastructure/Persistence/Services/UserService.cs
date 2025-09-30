using Application.Abstracts.Services;
using Application.DTOs.EmailDTOs;
using Application.DTOs.UserDTOs;
using Application.Shared;
using Application.Shared.Settings;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAppEmailService _appEmailService;
    //private readonly EmailPublisher _emailPublisher;

    public UserService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
          AppDbContext context,
          IHttpContextAccessor httpContextAccessor,
        IOptions<JWTSettings> jwtSetting,
        IAppEmailService appEmailService)
    /*EmailPublisher emailPublisher*/
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSetting.Value;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _appEmailService = appEmailService;
        //_emailPublisher = emailPublisher;
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
            SecurityStamp = Guid.NewGuid().ToString(),
            DocumentPath = string.Empty // boş string veririk
        };


        IdentityResult identityResult = await _userManager.CreateAsync(newUser, dto.Password);

        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors;
            StringBuilder errorMessage = new StringBuilder();
            foreach (var error in errors)
            {
                errorMessage.Append(error.Description + ";");
            }
            return new(errorMessage.ToString(), HttpStatusCode.BadRequest);
        }
        // 3. İstifadəçiyə "Patient" rolu verilir
        const string roleName = "Patient";
        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new IdentityRole(roleName));

        await _userManager.AddToRoleAsync(newUser, roleName);

        // 4. Patient məlumatı əlavə olunur
        var patient = new Patient
        {
            AppUserId = newUser.Id,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            MissedTurns = 0
        };
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        if (string.IsNullOrWhiteSpace(newUser.Email))
        {
            return new BaseResponse<TokenResponse>(
                "Email cannot be empty. Registration failed.",
                HttpStatusCode.BadRequest);
        }

        var confirmEmailLink = await GetEmailConfirmLink(newUser);

        // SMTP ilə email göndər
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return new BaseResponse<TokenResponse>(
            "User can't registered, something is wrong.",
            HttpStatusCode.BadRequest);

        }
        await _appEmailService.SendEmailAsync(newUser.Email,
                "Confirm your account",
                $"<p>Hello {newUser.FullName},</p>" +
                $"<p>Click the link below to confirm your account:</p>" +
                $"<a href='{confirmEmailLink}'>Confirm Email</a>");

        return new BaseResponse<TokenResponse>(
        "User registered successfully. For login, please check your email to confirm.",
        HttpStatusCode.Created
        );

    }
    public async Task<BaseResponse<TokenResponse>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return new BaseResponse<TokenResponse>("Invalid email or password", HttpStatusCode.Unauthorized);

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return new BaseResponse<TokenResponse>("Invalid email or password", HttpStatusCode.Unauthorized);

        // 3. Email təsdiqlənməyibsə (əgər şərt qoymaq istəsən)
        if (!user.EmailConfirmed)
        {
            return new BaseResponse<TokenResponse>("Please confirm your email before logging in", HttpStatusCode.Forbidden);
        }

        // 4. JWT token yaradılır
        var tokenResponse = await GenerateJwtToken(user);

        return new BaseResponse<TokenResponse>("Login successful", tokenResponse, HttpStatusCode.OK);
    }
    public async Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new BaseResponse<string>("User not found", HttpStatusCode.NotFound);

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new BaseResponse<string>($"Email confirmation failed: {errors}", HttpStatusCode.BadRequest);
        }

        return new BaseResponse<string>("Email confirmed successfully", HttpStatusCode.OK);
    }
    public async Task<BaseResponse<string>> UpdateUserAsync(UpdateDto dto)
    {
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
        if (user == null)
            return new BaseResponse<string>("User not found", HttpStatusCode.NotFound);

        // FullName dəyiş
        user.FullName = dto.FullName;

        // Email dəyiş
        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email;

        // PhoneNumber dəyiş
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;

        // Password dəyiş (təhlükəsizlik baxımından əslində köhnə parol da soruşulmalı idi)
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);

            if (!passResult.Succeeded)
            {
                var errors = string.Join("; ", passResult.Errors.Select(e => e.Description));
                return new BaseResponse<string>($"Password update failed: {errors}", HttpStatusCode.BadRequest);
            }
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
            return new BaseResponse<string>($"Update failed: {errors}", HttpStatusCode.BadRequest);
        }

        return new BaseResponse<string>("User updated successfully", HttpStatusCode.OK);
    }
    public async Task<BaseResponse<UserProfileDto>> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return new BaseResponse<UserProfileDto>("User not found", HttpStatusCode.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "NoRole",
            IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.Now
        };

        return new BaseResponse<UserProfileDto>(
            "User profile retrieved successfully",
            dto,
            HttpStatusCode.OK
        );
    }
    public async Task<BaseResponse<List<UserProfileDto>>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserProfileDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "NoRole",
                IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.Now
            });
        }

        return new BaseResponse<List<UserProfileDto>>(
            "All profiles retrieved successfully",
            result,
            HttpStatusCode.OK
        );
    }



    public async Task<BaseResponse<string>> AddRoleAsync(UserAddRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

        if (user is null)
        {
            return new BaseResponse<string>("User not found", false, HttpStatusCode.NotFound);
        }

        var seenRoleNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var roleNamesToAssign = new List<string>();

        foreach (var roleId in dto.RoleId.Distinct())
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null || string.IsNullOrWhiteSpace(role.Name))
            {
                return new BaseResponse<string>($"Role with ID '{roleId}' is invalid or has no name", false, HttpStatusCode.BadRequest);
            }
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                return new BaseResponse<string>($"User already has the role '{role.Name}'", false, HttpStatusCode.BadRequest);
            }
            // Eyni adda role təyin olunursa (məsələn, 2 müxtəlif ID eyni adda roldur)
            if (!seenRoleNames.Add(role.Name))
            {
                return new BaseResponse<string>($"Duplicate role '{role.Name}' detected in request", false, HttpStatusCode.BadRequest);
            }

            roleNamesToAssign.Add(role.Name);
        }

        // Əvvəlki rolları silirik
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                return new BaseResponse<string>($"Failed to remove existing roles: {errors}", false, HttpStatusCode.InternalServerError);
            }
        }

        // Yeni rolları təyin edirik
        foreach (var roleName in roleNamesToAssign)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BaseResponse<string>($"Failed to assign role '{roleName}': {errors}", false, HttpStatusCode.BadRequest);
            }
        }

        return new BaseResponse<string>(
            $"Roles updated successfully: {string.Join(", ", roleNamesToAssign)}",
            true,
            HttpStatusCode.OK
        );

    }


    public async Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return new BaseResponse<TokenResponse>("Invalid access token", HttpStatusCode.Unauthorized);

        var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null)
            return new BaseResponse<TokenResponse>("Invalid token claims", HttpStatusCode.Unauthorized);

        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return new BaseResponse<TokenResponse>("User not found", HttpStatusCode.NotFound);

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return new BaseResponse<TokenResponse>("Invalid refresh token", HttpStatusCode.Unauthorized);

        // Yeni JWT və RefreshToken yaradılır
        var newToken = await GenerateJwtToken(user);

        return new BaseResponse<TokenResponse>("Token refreshed successfully", newToken, HttpStatusCode.OK);
    }
    public async Task<BaseResponse<TokenResponse>> ResetPasswordAsync(UserResetPasswordDto dto)
    {
        // 1. Parolların uyğunluğunu yoxla
        if (dto.NewPassword != dto.ConfirmPassword)
            return new BaseResponse<TokenResponse>("Passwords do not match", HttpStatusCode.BadRequest);

        // 2. İstifadəçini tap
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return new BaseResponse<TokenResponse>("User not found", HttpStatusCode.NotFound);

        // 3. Şifrəni yenilə
        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new BaseResponse<TokenResponse>($"Password reset failed: {errors}", HttpStatusCode.BadRequest);
        }

        // 4. Uğurlu olduqda yeni token qaytar
        var tokenResponse = await GenerateJwtToken(user);

        return new BaseResponse<TokenResponse>("Password reset successfully", tokenResponse, HttpStatusCode.OK);
    }
    public async Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        // 1. User tap
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return new BaseResponse<string>("User not found", HttpStatusCode.NotFound);

        // 2. Token yarat
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // 3. Link formalaşdır (indi test üçün console-a)
        var resetLink = $"https://localhost:7046/api/Users/ResetPassword?email={dto.Email}&token={HttpUtility.UrlEncode(resetToken)}";

        Console.WriteLine("Password reset link: " + resetLink);

        // 4. Geri dönüş
        return new BaseResponse<string>("Password reset link generated successfully", HttpStatusCode.OK);
    }


    #region Privates
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
    private async Task<string> GetEmailConfirmLink(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = $"https://localhost:7149/api/User/ConfirmEmail?userId={user.Id}&token=" +
            $"{HttpUtility.UrlEncode(token)}";
        var decodeToken = HttpUtility.UrlDecode(token);
        var result = await _userManager.ConfirmEmailAsync(user,token);


        Console.WriteLine("Confirm email link : " + decodeToken);
        return link;
    }
    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // !!! expiry yoxlanmasın deyə false qoyuruq
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
    #endregion



}
