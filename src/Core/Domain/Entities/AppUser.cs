using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = null!;
    public string DocumentPath { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }

    public Guid DepartmentId { get; set; } = null!;
    public Guid MedicalServiceId { get; set; } = null!;
    public Doctor Doctor { get; set; }
    public Patient Patient { get; set; }
}
