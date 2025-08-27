using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Configurations;

//public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
//{
//    public void Configure(EntityTypeBuilder<Doctor> builder)
//    {
//        builder.HasKey(d => d.Id);

//        builder.Property(d => d.Specialization)
//               .IsRequired()
//               .HasMaxLength(200);

//        // Doctor -> AppUser (1:1)
//        builder.HasOne(d => d.AppUser)
//               .WithOne()
//               .HasForeignKey<Doctor>(d => d.AppUserId);

//        // Doctor -> QueueTickets (1:n)
//        builder.HasMany(d => d.QueueTickets)
//               .WithOne(q => q.Doctor)
//               .HasForeignKey(q => q.DoctorId)
//               .OnDelete(DeleteBehavior.Restrict);
//    }
//}
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);

        // 1:1 Doctor <-> AppUser
        builder.HasOne(d => d.AppUser)
               .WithOne(u => u.Doctor)
               .HasForeignKey<Doctor>(d => d.AppUserId)
               .IsRequired();

        // 1:N Doctor <-> Department
        builder.HasOne(d => d.Department)
               .WithMany(dep => dep.Doctors)
               .HasForeignKey(d => d.DepartmentId);
    }
}
