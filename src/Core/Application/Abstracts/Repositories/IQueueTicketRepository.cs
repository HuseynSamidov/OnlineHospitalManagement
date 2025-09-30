using Domain.Entities;
using Domain.Enums;

namespace Application.Abstracts.Repositories;

public interface IQueueTicketRepository : IRepository<QueueTicket>
{
    Task<QueueTicket?> GetActiveTicketByPatientAsync(Guid patientId, bool isTracking = false);
    Task<List<QueueTicket>> GetTicketsByDoctorAsync(Guid doctorId, QueueStatus? status = null, bool isTracking = false);
    Task<QueueTicket?> GetLastTicketByDoctorAsync(Guid doctorId, Guid serviceId, bool isTracking = false);
    Task<List<QueueTicket>> GetTicketsByServiceAsync(Guid serviceId, QueueStatus? status = null, bool isTracking = false);
}

