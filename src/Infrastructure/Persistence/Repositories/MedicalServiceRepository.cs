using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class MedicalServiceRepository : Repository<MedicalService>, IMedicalServiceRepository
    {
        private readonly AppDbContext _context;

        public MedicalServiceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Xüsusi metod: Service ilə birlikdə doctor-ları və department-i gətir
        public async Task<MedicalService?> GetServiceWithDoctorsAsync(Guid id, bool isTracking = false)
        {
            var query = _context.MedicalServices
                .Include(s => s.Doctors)
                .Include(s => s.Department)
                .Where(s => s.Id == id);

            if (!isTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }
    }
}
