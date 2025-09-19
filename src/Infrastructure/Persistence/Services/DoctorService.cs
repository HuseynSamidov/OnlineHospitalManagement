using Application.Abstracts.Services;
using Application.DTOs.DoctorDTOs;
using Application.Shared;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Persistence.Services;

public class DoctorService : IDoctorService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IFileUploadServices _fileUpload;
   // private readonly IAppEmailSender _emailSender;

    public DoctorService(UserManager<AppUser> userManager,
                         RoleManager<IdentityRole> roleManager,
                         IFileUploadServices fileUpload)
                        // IAppEmailSender emailSender)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _fileUpload = fileUpload;
      // _emailSender = emailSender;
    }

    public async Task<BaseResponse<string>> CreateDoctorAsync(DoctorRegisterDto dto)
    {
        // sənədi yükləyirik
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
           // DepartmentId = dto.DepartmentId,
           // MedicalServiceId = dto.MedicalServiceId,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(doctor, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            return new BaseResponse<string>(errors, HttpStatusCode.BadRequest);
        }

        // Doctor rolunu yoxlayırıq, yoxdursa yaradırıq
        if (!await _roleManager.RoleExistsAsync("Doctor"))
            await _roleManager.CreateAsync(new IdentityRole("Doctor"));

        await _userManager.AddToRoleAsync(doctor, "Doctor");

        // həkimə parol göndərilir
        //await _emailSender.SendEmailAsync(dto.Email,
        //    "Doctor Account Created",
        //    $"Salam {dto.FullName}, sizin həkim hesabınız yaradıldı. Login parolunuz: {password}");

        // uğurlu cavab
        return new BaseResponse<string>(
            "Doctor account created successfully",
            "Doctor account created successfully",
            HttpStatusCode.Created
        );
    }
}
