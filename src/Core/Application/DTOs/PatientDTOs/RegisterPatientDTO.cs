namespace Application.DTOs.PatientDTOs;

public class RegisterPatientDTO
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
