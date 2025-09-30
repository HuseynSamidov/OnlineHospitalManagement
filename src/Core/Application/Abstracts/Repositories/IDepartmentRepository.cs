using Domain.Entities;

namespace Application.Abstracts.Repositories;

public interface IDepartmentRepository:IRepository<Department>
{
    Task<ICollection<Department>> GetDepartmentsWithDoctorsAsync(Guid? parentCategoryId);
    Task<ICollection<Department>> GetByParentCategoryIdAsync(Guid? parentCategoryId);
    Task<bool> ExistsAsync(Guid id);
}