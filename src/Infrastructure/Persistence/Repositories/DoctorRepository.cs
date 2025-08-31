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
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Xüsusi metod: Doctor ilə birlikdə xidmətləri və department-i gətir
        public async Task<Doctor?> GetDoctorWithServicesAsync(Guid id, bool isTracking = false)
        {
            var query = _context.Doctors
                .Include(d => d.MedicalServices)
                .Include(d => d.Department)
                .Where(d => d.Id == id);

            if (!isTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }
    }
}
