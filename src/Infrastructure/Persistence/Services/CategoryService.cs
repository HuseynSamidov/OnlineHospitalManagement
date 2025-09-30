using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CategoryDTOs;
using Application.Shared;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Migrations;
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
    public async Task<BaseResponse<string>> CreateProcedureCategory(CreateProcedureDto dto)
    {
        var parentDepartment = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
        if(parentDepartment is null)
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
        return new("Procedure created succesfully",HttpStatusCode.Created); 

    }

}
