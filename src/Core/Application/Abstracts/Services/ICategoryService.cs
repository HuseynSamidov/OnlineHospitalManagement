using Application.DTOs.CategoryDTOs.Create;
using Application.DTOs.CategoryDTOs.Delete;
using Application.DTOs.CategoryDTOs.Update;
using Application.Shared;

namespace Application.Abstracts.Services;

public interface ICategoryService
{
    Task<BaseResponse<string>> CreateDepartmentCategory(CreateDepartmentDto dto);
    Task<BaseResponse<string>> CreateProcedureCategory(UpdateProcedureDto dto);
    Task<BaseResponse<string>> UpdateDepartmentAsync(UpdateDepartmentDto dto);
    Task<BaseResponse<string>> DeleteDepartmentAsync(DeleteDepartmentDto dto);
}
