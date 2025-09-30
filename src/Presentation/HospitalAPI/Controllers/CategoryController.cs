using Application.Abstracts.Services;
using Application.DTOs.CategoryDTOs.Create;
using Application.DTOs.CategoryDTOs.Delete;
using Application.DTOs.CategoryDTOs.Update;
using Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Category.MainCreate)]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
    {
        var result = await _categoryService.CreateDepartmentCategory(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Category.SubCreate)]
    public async Task<IActionResult> CreateProcedure([FromBody] UpdateProcedureDto dto)
    {
        var result = await _categoryService.CreateProcedureCategory(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Policy = Permissions.Category.MainUpdate)]
    public async Task<IActionResult> UpdateDepartment([FromBody] UpdateDepartmentDto dto)
    {
        var result = await _categoryService.UpdateDepartmentAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete]
    [Authorize(Policy = Permissions.Category.Delete)]
    public async Task<IActionResult> DeleteDepartment([FromBody] DeleteDepartmentDto dto)
    {
        var result = await _categoryService.DeleteDepartmentAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

}
