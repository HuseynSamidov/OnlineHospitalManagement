using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;


//public class PatientConfiguration : IEntityTypeConfiguration<Patient>
//{
//    public void Configure(EntityTypeBuilder<Patient> builder)
//    {
//        builder.HasKey(p => p.Id);

//        // Patient -> AppUser (1:1)
//        builder.HasOne(p => p.AppUser)
//               .WithOne()
//               .HasForeignKey<Patient>(p => p.AppUserId)
//               .OnDelete(DeleteBehavior.Cascade);

//        // Patient -> QueueTickets (1:n)
//        builder.HasMany(p => p.QueueTickets)
//               .WithOne(q => q.Patient)
//               .HasForeignKey(q => q.PatientId)
//               .OnDelete(DeleteBehavior.Restrict);
//    }
//}


public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        // 1:1 Patient <-> AppUser
        builder.HasOne(p => p.AppUser)
               .WithOne(u => u.Patient)
               .HasForeignKey<Patient>(p => p.AppUserId)
               .IsRequired();
    }
}
