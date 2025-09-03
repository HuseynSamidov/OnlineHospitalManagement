using Application.DTOs.PatientDTOs;
using Domain.Entities;

namespace Application.Abstracts.Services;

public interface IPatientService
{
    Task<Patient?> GetByIdAsync(Guid id);
    Task<List<Patient>> GetAllAsync();  
    Task<Patient> UpdateAsync(UpdatePatientDTO dto);
    Task<bool> DeleteAsync(Guid id);

    Task<Patient?> GetPatientWithTicketsAsync(Guid id);
    Task<bool> HasActiveTicketAsync(Guid patientId);
}
