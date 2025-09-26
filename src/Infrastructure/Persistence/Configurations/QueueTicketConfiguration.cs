using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



public class QueueTicketConfiguration : IEntityTypeConfiguration<QueueTicket>
{
    public void Configure(EntityTypeBuilder<QueueTicket> builder)
    {
        builder.HasKey(q => q.Id);

        // Patient - QueueTickets
        builder.HasOne(q => q.Patient)
            .WithMany(p => p.QueueTickets)
            .HasForeignKey(q => q.PatientId)
            .OnDelete(DeleteBehavior.Restrict); // ❌ Cascade yox

        // Doctor - QueueTickets
        builder.HasOne(q => q.Doctor)
            .WithMany(d => d.QueueTickets)
            .HasForeignKey(q => q.DoctorId)
            .OnDelete(DeleteBehavior.Restrict); // ❌ Cascade yox

        // MedicalService - QueueTickets
        builder.HasOne(q => q.Procedure)
            .WithMany(s => s.QueueTickets)
            .HasForeignKey(q => q.ProcedureId)
            .OnDelete(DeleteBehavior.Cascade); // ✅ Burada Cascade qalsın

        builder.Property(q => q.Number)
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.Status)
            .IsRequired();
    }
}
