using Domain.Entities;

namespace Application.Abstracts.Repositories;

public interface IProcedureRepository : IRepository<Procedure>
{
    Task<Procedure?> GetServiceWithDoctorsAsync(Guid id, bool isTracking = false);
}
