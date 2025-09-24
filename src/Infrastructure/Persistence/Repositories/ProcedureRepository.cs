using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class ProcedureRepository : Repository<Procedure>, IProcedureRepository
{
    private readonly AppDbContext _context;

    public ProcedureRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    // Xüsusi metod: Service ilə birlikdə doctor-ları və department-i gətir
    public async Task<Procedure?> GetServiceWithDoctorsAsync(Guid id, bool isTracking = false)
    {
        var query = _context.Procedures
            .Include(s => s.Doctors)
            .Include(s => s.Department)
            .Where(s => s.Id == id);

        if (!isTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}
