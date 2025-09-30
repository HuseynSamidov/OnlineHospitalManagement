using Application.Abstracts.Services;
using Application.DTOs.RoleDTOs;
using Application.Shared;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Persistence.Services;

public class RoleService : IRoleService
{
    private RoleManager<IdentityRole> _roleManager { get; }

    public RoleService(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<BaseResponse<string?>> CreateRoleAsync(RoleCreateDto dto)
    {
        // Rolun artıq olub olmadığını yoxla
        var existingRole = await _roleManager.FindByNameAsync(dto.Name);
        if (existingRole != null)
        {
            return new BaseResponse<string?>("Role already exists", HttpStatusCode.BadRequest);
        }

        var newRole = new IdentityRole(dto.Name);
        var result = await _roleManager.CreateAsync(newRole);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return new BaseResponse<string?>(errorMessage, HttpStatusCode.BadRequest);
        }

        // İcazələri (Permission) rola əlavə et
        foreach (var permission in dto.PermissionList)
        {
            await _roleManager.AddClaimAsync(newRole, new System.Security.Claims.Claim("Permission", permission));
        }

        return new BaseResponse<string?>("Role created successfully", true, HttpStatusCode.Created);
    }

    public async Task<BaseResponse<string?>> UpdateRoleAsync(string roleId, RoleCreateDto dto)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return new BaseResponse<string?>("Role not found", HttpStatusCode.NotFound);
        }

        role.Name = dto.Name;
        var updateResult = await _roleManager.UpdateAsync(role);
        if (!updateResult.Succeeded)
        {
            var errorMessage = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return new BaseResponse<string?>(errorMessage, HttpStatusCode.BadRequest);
        }

        // Mövcud permission claim-ləri sil
        var currentClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var claim in currentClaims.Where(c => c.Type == "Permission"))
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }

        // Yeni permission-ləri əlavə et
        foreach (var permission in dto.PermissionList)
        {
            await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permission));
        }

        return new BaseResponse<string?>("Role updated successfully", role.Id, HttpStatusCode.OK);
    }
}
