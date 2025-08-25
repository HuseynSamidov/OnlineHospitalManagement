using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class ServiceCategory : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }

    // Relations
    public ICollection<Service> Services { get; set; }
}
