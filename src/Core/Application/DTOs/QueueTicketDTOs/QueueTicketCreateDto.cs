using Domain.Enums;

namespace Application.DTOs.QueueTicketDTOs;

public record QueueTicketCreateDto(
    Guid PatientId,
    Guid MedicalServiceId
);