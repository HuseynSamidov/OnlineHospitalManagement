using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourProject.Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // One-to-many: Department -> Doctors
            builder.HasMany(d => d.Doctors)
                   .WithOne(doc => doc.Department)
                   .HasForeignKey(doc => doc.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ParentCategory)
             .WithMany(c => c.Procedures)
             .HasForeignKey(c => c.ParentCategoryId)
             .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
