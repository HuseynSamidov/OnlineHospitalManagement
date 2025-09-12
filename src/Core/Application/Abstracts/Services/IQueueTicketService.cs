using Application.DTOs.QueueTicketDTOs;
using Application.Shared;

namespace Application.Abstracts.Services;

public interface IQueueTicketService
{
    Task<BaseResponse<QueueTicketGetDto>> GetByIdAsync(Guid id);
    Task<BaseResponse<QueueTicketGetDto>> CreateAsync(QueueTicketCreateDto dto);

    Task<BaseResponse<bool>> UpdateStatusAsync(QueueTicketUpdateStatusDto dto);
    Task ProcessMissedTicketAsync(Guid ticketId);

    Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetByPatientIdAsync(Guid patientId);
    Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetByMedicalServiceIdAsync(Guid medicalServiceId);

    // Aktiv ticket-lər (Waiting və Called olanlar)
    Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetActiveTicketsAsync();
}

