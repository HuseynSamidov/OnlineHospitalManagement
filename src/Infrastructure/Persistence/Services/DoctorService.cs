using Application.Abstracts.Services;
using Application.DTOs.DoctorDTOs;
using Application.Shared;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Services;

public class DoctorService : IDoctorService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IFileUploadServices _fileUpload;

    public DoctorService(UserManager<AppUser> userManager,
                         RoleManager<IdentityRole> roleManager,
                         IFileUploadServices fileUpload)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _fileUpload = fileUpload;
    }

    public async Task<BaseResponse<string>> CreateDoctorAsync(DoctorRegisterDto dto)
    {
        // sənədi serverə upload edirik
        var documentPath = await _fileUpload.UploadAsync(dto.Document);

        // random parol yaradılır
        var password = Guid.NewGuid().ToString("N").Substring(0, 8);

        var doctor = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            Gender = dto.Gender,
            DocumentPath = documentPath,
            DepartmentId = dto.DepartmentId,
            MedicalServiceId = dto.MedicalServiceId,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(doctor, password);
        if (!result.Succeeded)
            return BaseResponse<string>.Fail(string.Join(", ", result.Errors.Select(x => x.Description)));

        // Doctor roluna əlavə edirik
        if (!await _roleManager.RoleExistsAsync("Doctor"))
            await _roleManager.CreateAsync(new IdentityRole("Doctor"));

        await _userManager.AddToRoleAsync(doctor, "Doctor");

        // burda mail göndərilir -> həkimə parolu çatdırmaq üçün
        // IAppEmailSender.SendEmailAsync(dto.Email, "Your Account Password", $"Password: {password}");

        return BaseResponse<string>.Success("Doctor account created successfully");
    }
}
