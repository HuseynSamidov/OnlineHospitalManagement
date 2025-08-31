using Application.Abstracts.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class QueueTicketRepository : Repository<QueueTicket>, IQueueTicketRepository
    {
        private readonly AppDbContext _context;

        public QueueTicketRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Xüsusi metod: Xəstənin aktiv növbəsini gətir
        public async Task<QueueTicket?> GetActiveTicketByPatientAsync(Guid patientId, bool isTracking = false)
        {
            var query = _context.QueueTickets
                .Include(q => q.Doctor)
                .Include(q => q.Patient)
                .Include(q => q.Service)
                .Where(q => q.PatientId == patientId && q.Status == QueueStatus.Waiting);

            if (!isTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        // Xüsusi metod: Həkimin növbələrini status üzrə gətir
        public async Task<List<QueueTicket>> GetTicketsByDoctorAsync(Guid doctorId, QueueStatus? status = null, bool isTracking = false)
        {
            var query = _context.QueueTickets
                .Include(q => q.Patient)
                .Include(q => q.Service)
                .Where(q => q.DoctorId == doctorId);

            if (status.HasValue)
                query = query.Where(q => q.Status == status.Value);

            if (!isTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
