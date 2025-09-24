using Domain.Enums;

namespace Domain.Entities;

public class QueueTicket : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; }

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; }

    public Guid ProcedureId { get; set; }
    public Procedure Procedure { get; set; }

    public int Number { get; set; } // Sıradakı nömrəsi
    public DateTime ScheduledAt { get; set; } //ne zaman pasient daxil olmalidir
    public QueueStatus Status { get; set; } = QueueStatus.Waiting; // Waiting, Called, Missed, Completed
}


