namespace Application.DTOs.UserDTOs;

public record UserAddRoleDto
(
    Guid UserId,

    List<Guid>? RoleId
);
