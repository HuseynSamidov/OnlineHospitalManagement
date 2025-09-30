using Domain.Entities;


namespace Application.Abstracts.Repositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    // extra methods can be added, stay tuned...
    Task<Department?> GetDoctorWithServicesAsync(Guid doctorId);
}
