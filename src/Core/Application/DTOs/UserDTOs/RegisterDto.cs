namespace Application.DTOs.UserDTOs;

public record RegisterDto(
    string FullName,
    string Email,
    string Password,
    DateTime DateOfBirth, // artıq null deyil
    string Gender
);


