using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstracts.Repositories;

public interface IMedicalServiceRepository : IRepository<MedicalService>
{
    Task<MedicalService?> GetServiceWithDoctorsAsync(Guid id, bool isTracking = false);
}
