namespace Persistence.Configurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
{
    public void Configure(EntityTypeBuilder<Procedure> builder)
    {
        builder.HasKey(ms => ms.Id);

        builder.Property(ms => ms.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(ms => ms.Description)
               .HasMaxLength(500);

        // Many-to-one: Procedure -> Department
        builder.HasOne(ms => ms.Department)
               .WithMany(d => d.Procedure) 
               .HasForeignKey(ms => ms.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many: Procedure <-> Doctor
        builder.HasMany(ms => ms.Doctors)
               .WithMany(d => d.MedicalServices);
    }
}
