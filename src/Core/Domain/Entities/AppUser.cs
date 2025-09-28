using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = null!;
    public string DocumentPath { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; } 
    public Guid MedicalServiceId { get; set; } 
    public Doctor Doctor { get; set; }
    public Patient Patient { get; set; }
}
