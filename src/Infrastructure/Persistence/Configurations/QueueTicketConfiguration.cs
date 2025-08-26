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

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
               .IsRequired()
               .HasMaxLength(100);

        // One-to-many: Department -> Doctors
        builder.HasMany(d => d.Doctors)
               .WithOne(doc => doc.Department)
               .HasForeignKey(doc => doc.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict);

        // One-to-many: Department -> MedicalServices
        builder.HasMany(d => d.MedicalService)
               .WithOne(ms => ms.ServiceCategory)
               .HasForeignKey(ms => ms.ServiceCategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
