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

    public string Specialization { get; set; }
    public int ExperienceYears { get; set; }

    // Relations
    public ICollection<Service> Services { get; set; }
    public ICollection<QueueTicket> QueueTickets { get; set; }
}
