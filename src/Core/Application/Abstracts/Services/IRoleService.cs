using Application.DTOs.RoleDTOs;
using Application.Shared;

namespace Application.Abstracts.Services;

public interface IRoleService
{
    Task<BaseResponse<string?>> CreateRoleAsync(RoleCreateDto dto);

    Task<BaseResponse<string?>> UpdateRoleAsync(string RoleId, RoleCreateDto dto);
}
