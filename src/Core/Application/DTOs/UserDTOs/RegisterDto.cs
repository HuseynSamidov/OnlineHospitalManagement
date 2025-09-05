namespace Application.DTOs.UserDTOs;

public record RegisterDto(
    string FullName,
    string Email,
    string Password,
    string Role, 
    DateTime? DateOfBirth = null, // Patient üçün
    string? Gender = null,        // Patient üçün
    string? Specialization = null, // Doctor üçün
    Guid? DepartmentId = null      // Doctor üçün
);

