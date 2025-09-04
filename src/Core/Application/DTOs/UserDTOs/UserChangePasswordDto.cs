namespace Application.DTOs.UserDTOs;

public record UserChangePasswordDto
(
 string CurrentPassword,
 string NewPassword,
 string ConfirmPassword
);
