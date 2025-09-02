using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstracts.Services;

public interface IDoctorService
{
    // CRUD metodları
    Task<Doctor?> GetByIdAsync(Guid id);
    Task<List<Doctor>> GetAllAsync();
    Task<Doctor> CreateAsync(Doctor doctor);
    Task<Doctor> UpdateAsync(Doctor doctor);
    Task<bool> DeleteAsync(Guid id);
    /// Verilmiş department-ə aid olan bütün həkimləri gətirir
    Task<List<Doctor>> GetDoctorsByDepartmentAsync(Guid departmentId);

    /// Müəyyən tibbi xidmət üzrə çalışan həkimləri gətirir
    Task<List<Doctor>> GetDoctorsByServiceAsync(Guid serviceId);
}
