using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;


namespace Application.Abstracts.Repositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    // extra methods can be added, stay tuned...
    Task<Doctor?> GetDoctorWithServicesAsync(Guid id, bool isTracking = false);
}
