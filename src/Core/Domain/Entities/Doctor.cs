namespace Domain.Entities;

public class Doctor : BaseEntity
{
    // Identity default olaraq string Id istifadə edir
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }

    // Həkim yalnız bir Department-ə bağlı ola bilər
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }

    // İxtisası (məsələn: kardioloq, neyrocərrah və s.)
    public string Specialization { get; set; }

    // Həkimin təqdim etdiyi tibbi xidmətlər
    public ICollection<MedicalService> MedicalServices { get; set; } = new List<MedicalService>();

    // Həkimin qəbul növbələri
    public ICollection<QueueTicket> QueueTickets { get; set; } = new List<QueueTicket>();
}
