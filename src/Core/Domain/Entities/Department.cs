using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; }

    // Bir Department-də çox həkim ola bilər
    public ICollection<Doctor> Doctors { get; set; }
    public ICollection<Procedure> Procedure { get; set; }
}
