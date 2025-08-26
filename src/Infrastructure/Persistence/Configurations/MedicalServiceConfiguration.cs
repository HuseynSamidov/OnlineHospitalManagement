namespace Persistence.Configurations;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MedicalServiceConfiguration : IEntityTypeConfiguration<MedicalService>
{
    public void Configure(EntityTypeBuilder<MedicalService> builder)
    {
        builder.HasKey(ms => ms.Id);

        builder.Property(ms => ms.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(ms => ms.Description)
               .HasMaxLength(500);

        // Many-to-one: MedicalService -> Department
        builder.HasOne(ms => ms.ServiceCategory)
               .WithMany(d => d.MedicalService)
               .HasForeignKey(ms => ms.ServiceCategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many: MedicalService <-> Doctor
        builder.HasMany(ms => ms.Doctors)
               .WithMany(d => d.MedicalServices);
    }
}
