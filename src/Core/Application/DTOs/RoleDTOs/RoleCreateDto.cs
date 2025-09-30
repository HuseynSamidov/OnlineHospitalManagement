namespace Application.DTOs.RoleDTOs;

public record RoleCreateDto
{
    public string Name { get; init; } = null!;

    public List<string> PermissionList { get; set; }
}
