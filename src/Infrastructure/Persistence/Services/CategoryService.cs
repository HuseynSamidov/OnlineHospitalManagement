using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CategoryDTOs.Create;
using Application.DTOs.CategoryDTOs.Delete;
using Application.DTOs.CategoryDTOs.Update;
using Application.Shared;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Migrations;
using System.Linq.Expressions;
using System.Net;

namespace Persistence.Services;

public class CategoryService : ICategoryService
{
    private readonly IDepartmentRepository _departmentRepository;

    public CategoryService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }
    public async Task<BaseResponse<string>> CreateDepartmentCategory(CreateDepartmentDto dto)
    {
        var existDepartment = await _departmentRepository
        .GetByFiltered(d => d.Name.ToLower() == dto.DepartmentName.ToLower())
        .AnyAsync();

        if (existDepartment)
        {
            return new("Bu adda department artıq mövcuddur.", HttpStatusCode.BadRequest);
        }

        var department = new Department
        {
            Name = dto.DepartmentName,
            Description = dto.Description

        };
        await _departmentRepository.AddAsync(department);
        await _departmentRepository.SaveChangeAsync();
        return new("Department created succesfully", HttpStatusCode.Created);
    }
    public async Task<BaseResponse<string>> CreateProcedureCategory(UpdateProcedureDto dto)
    {
        var parentDepartment = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
        if (parentDepartment is null)
            return new(HttpStatusCode.BadRequest);

        var procedure = new Department
        {
            Id = Guid.NewGuid(), // yeni ID
            Name = dto.Name,
            Description = dto.Description,
            ParentCategoryId = dto.DepartmentId
        };

        await _departmentRepository.AddAsync(procedure);
        await _departmentRepository.SaveChangeAsync();
        return new("Procedure created succesfully", HttpStatusCode.Created);

    }
    public async Task<BaseResponse<string>> UpdateDepartmentAsync(UpdateDepartmentDto dto)
    {
        var department = await _departmentRepository.GetByIdAsync(dto.Id);
        if (department is null)
        {
            return new("Department cannot be found", HttpStatusCode.BadRequest);
        }
        department.Name = dto.NewName;

        _departmentRepository.Update(department);
        await _departmentRepository.SaveChangeAsync();
        return new("Department updated succesfully", HttpStatusCode.OK);
    }
    public async Task<BaseResponse<string>> DeleteDepartmentAsync(DeleteDepartmentDto dto)
    {
        var department = await _departmentRepository.GetByIdAsync(dto.Id);
        if (department is null)
            return new("Department do not exist", HttpStatusCode.NotFound);

        Expression<Func<Department, object>>[] includes = { d => d.Procedures };

        var hasProcedures = await _departmentRepository
            .GetAllFiltered(
                d => d.Id == dto.Id,
                include: includes,
                isTracking: false
            )
            .AnyAsync(d => d.Procedures.Any());


        if (hasProcedures)
            return new("Cannot delete department. There are some datas on it.", HttpStatusCode.BadRequest);


        _departmentRepository.Delete(department);
        await _departmentRepository.SaveChangeAsync();
        return new("Department deleted succesfully", HttpStatusCode.OK);

    }
}
