using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstracts.Services;

public interface IQueueTicketService
{
    Task<QueueTicket> CreateAsync(Guid patientId, Guid medicalServiceId);

    Task<bool> UpdateStatusAsync(Guid ticketId, QueueStatus newStatus);
    Task ProcessMissedTicketAsync(Guid ticketId);
    Task<IEnumerable<QueueTicket>> GetByPatientIdAsync(Guid patientId);

    Task<IEnumerable<QueueTicket>> GetByMedicalServiceIdAsync(Guid medicalServiceId);

    // Aktiv ticket-lər (Waiting və Called olanlar)
    Task<IEnumerable<QueueTicket>> GetActiveTicketsAsync();
    Task<QueueTicket?> GetByIdAsync(Guid id);
}
