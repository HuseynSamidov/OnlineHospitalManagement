using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class AppUser : IdentityUser
{
    public required string FullName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public int Age { get; set; }

    public Doctor Doctor { get; set; }
    public Patient Patient { get; set; }
}
