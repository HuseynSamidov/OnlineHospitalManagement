using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    private readonly AppDbContext _context;
    public DepartmentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Departments
            .AnyAsync(x => x.Id == id);
    }

    public async Task<ICollection<Department>> GetByParentCategoryIdAsync(Guid? parentCategoryId)
    {
        return await _context.Departments
            .Where(d=>(d.ParentCategoryId == Guid.Empty ||d.ParentCategoryId == null))
            .Include(d => d.Procedures)
                    .ThenInclude(p=>p.Procedures)
            .ToListAsync();
    }

    public async Task<ICollection<Department>> GetDepartmentsWithDoctorsAsync(Guid? parentCategoryId)
    {
        return await _context.Departments
            .Where(d => (d.ParentCategoryId == Guid.Empty || d.ParentCategoryId == null))
            .Include(d => d.Procedures)
                    .ThenInclude(p => p.DoctorId)
            .ToListAsync();
    }
}
