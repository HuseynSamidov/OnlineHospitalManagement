using Application.Abstracts.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstracts.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    // extra methods can be added, stay tuned...
    Task<Patient?> GetPatientWithTicketsAsync(Guid id, bool isTracking = false);
}




