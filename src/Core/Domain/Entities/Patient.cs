namespace Domain.Entities;

public class Patient : BaseEntity
{
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }

    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public int MissedTurns { get; set; } = 0; // neçə dəfə növbəsini qaçırıb
    public DateTime? BlockedUntil { get; set; } // 3 dəfə qaçırsa, 1 həftəlik blok tarixi

    // Relations
    public ICollection<QueueTicket> QueueTickets { get; set; }
}
