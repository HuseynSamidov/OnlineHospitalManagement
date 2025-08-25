using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; }

    // Relations
    public ICollection<Doctor> Doctors { get; set; }
}
