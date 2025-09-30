using Application.Abstracts.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class QueueTicketRepository : Repository<QueueTicket>, IQueueTicketRepository
{
    private readonly AppDbContext _context;

    public QueueTicketRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<QueueTicket?> GetActiveTicketByPatientAsync(Guid patientId, bool isTracking = false)
    {
        var query = _context.QueueTickets.AsQueryable();
        if (!isTracking)
            query = query.AsNoTracking();

        return await query
            .FirstOrDefaultAsync(q => q.PatientId == patientId
                                   && (q.Status == QueueStatus.Waiting || q.Status == QueueStatus.Called));
    }

    public async Task<List<QueueTicket>> GetTicketsByDoctorAsync(Guid doctorId, QueueStatus? status = null, bool isTracking = false)
    {
        var query = _context.QueueTickets.AsQueryable();
        if (!isTracking)
            query = query.AsNoTracking();

        if (status.HasValue)
            query = query.Where(q => q.DoctorId == doctorId && q.Status == status.Value);
        else
            query = query.Where(q => q.DoctorId == doctorId);

        return await query.ToListAsync();
    }

    public async Task<QueueTicket?> GetLastTicketByDoctorAsync(Guid doctorId, Guid serviceId, bool isTracking = false)
    {
        var query = _context.QueueTickets.AsQueryable();

        if (!isTracking)
            query = query.AsNoTracking();

        return await query
            .Where(q => q.ProcedureId == serviceId && q.DoctorId == doctorId)
            .OrderByDescending(q => q.ScheduledAt)
            .FirstOrDefaultAsync();
    }


    public async Task<List<QueueTicket>> GetTicketsByServiceAsync(Guid serviceId, QueueStatus? status = null, bool isTracking = false)
    {
        var query = _context.QueueTickets.AsQueryable();
        if (!isTracking)
            query = query.AsNoTracking();

        if (status.HasValue)
            query = query.Where(q => q.ProcedureId == serviceId && q.Status == status.Value);
        else
            query = query.Where(q => q.ProcedureId == serviceId);

        return await query.ToListAsync();
    }
}
