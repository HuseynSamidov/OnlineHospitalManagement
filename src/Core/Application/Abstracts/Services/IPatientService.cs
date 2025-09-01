using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstracts.Services;

public interface IPatientService
{
    Task<Patient?> GetByIdAsync(Guid id);
    Task<List<Patient>> GetAllAsync();  
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient> UpdateAsync(Patient patient);
    Task<bool> DeleteAsync(Guid id);

    Task<Patient?> GetPatientWithTicketsAsync(Guid id);
    Task<bool> HasActiveTicketAsync(Guid patientId);
}
