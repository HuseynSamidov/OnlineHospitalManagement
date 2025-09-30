using Application.Abstracts.Services;
using Application.DTOs.RoleDTOs;
using Application.Shared;
using Application.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HospitalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private IRoleService _roleService { get; }
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("Permissions")]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllPermissions()
        {
            var permissions = PermissionHelper.GetAllPermissions();
            return Ok(permissions);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create(RoleCreateDto dto)
        {
            var result = await _roleService.CreateRoleAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }


    }
}
