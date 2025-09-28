using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDTOs;

public class RegisterDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string Gender { get; set; }
}


