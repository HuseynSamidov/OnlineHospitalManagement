namespace Domain.Entities;

public class MedicalService : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }

    public Guid DoctorId { get; set; }
    // Relations
    public ICollection<QueueTicket> QueueTickets { get; set; }
    public ICollection<Doctor> Doctors { get; set; }
}
