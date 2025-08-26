using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class QueueTicket : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public Guid ServiceId { get; set; }
    public MedicalService Service { get; set; }

    public int Number { get; set; } // Sıradakı nömrəsi
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public QueueStatus Status { get; set; } = QueueStatus.Waiting; // Waiting, Called, Missed, Completed
}


