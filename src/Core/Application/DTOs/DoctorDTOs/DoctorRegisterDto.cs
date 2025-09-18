using Microsoft.AspNetCore.Http;

namespace Application.DTOs.DoctorDTOs;

public record DoctorRegisterDto
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Gender { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid MedicalServiceId { get; set; }

    // Mütləq sənəd (məs: pdf, jpg və ya scan edilmiş şəkil)
    public IFormFile Document { get; set; }
}

