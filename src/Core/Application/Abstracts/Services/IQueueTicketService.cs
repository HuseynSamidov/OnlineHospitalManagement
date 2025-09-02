using Domain.Entities;
using Domain.Enums;

namespace Application.Abstracts.Services;

public interface IQueueTicketService
{
    Task<QueueTicket?> GetByIdAsync(Guid id);
    Task<QueueTicket> CreateAsync(Guid patientId, Guid medicalServiceId);

    Task<bool> UpdateStatusAsync(Guid ticketId, QueueStatus newStatus);
    Task ProcessMissedTicketAsync(Guid ticketId);
    Task<IEnumerable<QueueTicket>> GetByPatientIdAsync(Guid patientId);

    Task<IEnumerable<QueueTicket>> GetByMedicalServiceIdAsync(Guid medicalServiceId);

    // Aktiv ticket-lər (Waiting və Called olanlar)
    Task<IEnumerable<QueueTicket>> GetActiveTicketsAsync();
}
