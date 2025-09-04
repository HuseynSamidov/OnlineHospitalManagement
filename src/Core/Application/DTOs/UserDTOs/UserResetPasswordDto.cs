namespace Application.DTOs.UserDTOs;

public record UserResetPasswordDto
(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword
);
