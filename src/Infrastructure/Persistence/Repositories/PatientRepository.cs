using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PatientRepository : Repository<Patient>, IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    // Xüsusi metod: Patient ilə birlikdə QueueTickets-i gətir
    public async Task<Patient?> GetPatientWithTicketsAsync(Guid id, bool isTracking = false)
    {
        var query = _context.Patients
            .Include(p => p.QueueTickets)
            .Where(p => p.Id == id);

        if (!isTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}
