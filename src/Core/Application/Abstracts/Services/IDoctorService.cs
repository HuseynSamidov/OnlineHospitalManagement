using Application.DTOs.DoctorDTOs;
using Application.Shared;

namespace Application.Abstracts.Services;

public interface IDoctorService
{
    Task<BaseResponse<string>> CreateDoctorAsync(DoctorRegisterDto dto);
}
