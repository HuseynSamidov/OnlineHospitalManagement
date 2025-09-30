using Application.Abstracts.Services;
using Application.DTOs.CategoryDTOs.Create;
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
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
    {
        var result = await _categoryService.CreateDepartmentCategory(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProcedure([FromBody] UpdateProcedureDto dto)
    {
        var result = await _categoryService.CreateProcedureCategory(dto);
        return StatusCode((int)result.StatusCode, result);
    }

}
