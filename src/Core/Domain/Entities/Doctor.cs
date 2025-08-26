using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Doctor : BaseEntity
{
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; }

    // Həkim yalnız bir Department-ə bağlı ola bilər
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }

    public string Specialization { get; set; }

    public ICollection<MedicalService> MedicalServices { get; set; }
    public ICollection<QueueTicket> QueueTickets { get; set; }
}
