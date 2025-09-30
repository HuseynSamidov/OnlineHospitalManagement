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
        public async Task<Department?> GetDoctorWithServicesAsync(Guid doctorId)
        {
            return await _context.Doctors
                .Where(d=>d.Id == doctorId)
                .Include(d=>d.Procedure)
                .Select(d => d.Procedure)
                .FirstOrDefaultAsync();
        }
    }
}
