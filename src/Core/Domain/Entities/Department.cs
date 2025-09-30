namespace Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Guid? ParentCategoryId { get; set; }
    public Department? ParentCategory { get; set; }
    // Bir Department-də çox həkim ola bilər
    public ICollection<Doctor> Doctors { get; set; }
    public Guid DoctorId { get; set; }
    public ICollection<QueueTicket> Tickets { get; set; }
    public ICollection<Department> Procedures { get; set; } = new List<Department>();
}
