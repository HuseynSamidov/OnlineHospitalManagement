using Domain.Enums;

namespace Application.DTOs.QueueTicketDTOs;

public record QueueTicketGetDto(
Guid Id,
Guid PatientId,
Guid MedicalServiceId,
QueueStatus Status,
DateTime? CreatedAt,
 int Number
);

