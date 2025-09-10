using Domain.Enums;

namespace Application.DTOs.QueueTicketDTOs;

public record QueueTicketUpdateStatusDto(
Guid TicketId,
QueueStatus NewStatus
);
