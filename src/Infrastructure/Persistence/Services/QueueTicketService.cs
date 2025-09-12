using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.QueueTicketDTOs;
using Application.Shared;
using Domain.Entities;
using Domain.Enums;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using System.Net;

namespace Persistence.Services;

public class QueueTicketService : IQueueTicketService
{
    private readonly IQueueTicketRepository _queueTicketRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IMedicalServiceRepository _medicalServiceRepository;

    public QueueTicketService(
        IQueueTicketRepository queueTicketRepository,
        IPatientRepository patientRepository,
        IMedicalServiceRepository medicalServiceRepository)
    {
        _queueTicketRepository = queueTicketRepository;
        _patientRepository = patientRepository;
        _medicalServiceRepository = medicalServiceRepository;
    }

    public async Task<BaseResponse<QueueTicketGetDto>> GetByIdAsync(Guid id)
    {
        var ticket = await _queueTicketRepository.GetByIdAsync(id);
        if (ticket is null)
            return new BaseResponse<QueueTicketGetDto>("Ticket not found", HttpStatusCode.NotFound);

        var dto = new QueueTicketGetDto(
            ticket.Id,
            ticket.PatientId,
            ticket.ServiceId,
            ticket.Status,
            ticket.ScheduledAt,
            ticket.Number
        );

        return new BaseResponse<QueueTicketGetDto>("Ticket found", dto, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<QueueTicketGetDto>> CreateAsync(QueueTicketCreateDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
        if (patient == null)
            return new BaseResponse<QueueTicketGetDto>("Patient not found", HttpStatusCode.NotFound);

        var service = await _medicalServiceRepository.GetByIdAsync(dto.MedicalServiceId);
        if (service == null)
            return new BaseResponse<QueueTicketGetDto>("Service not found", HttpStatusCode.NotFound);

        // Son ticket-ı tap
        var lastTicket = (await _queueTicketRepository
            .GetTicketsByDoctorAsync(service.DoctorId))
            .OrderByDescending(t => t.ScheduledAt)
            .FirstOrDefault();

        DateTime scheduledAt = lastTicket == null
            ? DateTime.UtcNow
            : lastTicket.ScheduledAt.AddMinutes(30);

        var ticket = new QueueTicket
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            ServiceId = dto.MedicalServiceId,
            DoctorId = service.DoctorId,
            Number = lastTicket?.Number + 1 ?? 1,
            ScheduledAt = scheduledAt,
            Status = QueueStatus.Waiting
        };

        await _queueTicketRepository.AddAsync(ticket);

        // Hangfire ilə yoxlama task-ı schedule et (30 dəq sonra)
        BackgroundJob.Schedule<IQueueTicketService>(
            x => x.ProcessMissedTicketAsync(ticket.Id),
            scheduledAt.AddMinutes(30) - DateTime.UtcNow
        );

        var result = new QueueTicketGetDto(
            ticket.Id,
            ticket.PatientId,
            ticket.ServiceId,
            ticket.Status,
            ticket.ScheduledAt,
            ticket.Number
        );

        return new BaseResponse<QueueTicketGetDto>(
            "Ticket created successfully",
            result,
            HttpStatusCode.OK
        );
    }

    public async Task<BaseResponse<bool>> UpdateStatusAsync(QueueTicketUpdateStatusDto dto)
    {
        var ticket = await _queueTicketRepository.GetByIdAsync(dto.TicketId);
        if (ticket is null)
            return new BaseResponse<bool>("Ticket not found", false, HttpStatusCode.NotFound);

        ticket.Status = dto.NewStatus;
        await _queueTicketRepository.UpdateAsync(ticket);

        return new BaseResponse<bool>("Status updated", true, HttpStatusCode.OK);
    }

    public async Task ProcessMissedTicketAsync(Guid ticketId)
    {
        var ticket = await _queueTicketRepository.GetByIdAsync(ticketId);
        if (ticket == null || ticket.Status != QueueStatus.Waiting)
            return;

        if (DateTime.UtcNow >= ticket.ScheduledAt.AddMinutes(30))
        {
            ticket.Status = QueueStatus.Missed;
            await _queueTicketRepository.UpdateAsync(ticket);

            var patient = await _patientRepository.GetByIdAsync(ticket.PatientId);
            if (patient != null)
            {
                patient.MissedTurns += 1;

                if (patient.MissedTurns >= 3)
                {
                    patient.BlockedUntil = DateTime.UtcNow.AddDays(7);
                    patient.MissedTurns = 0;
                }

                await _patientRepository.UpdateAsync(patient);
            }
        }
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetByPatientIdAsync(Guid patientId)
    {
        var tickets = await _queueTicketRepository.GetActiveTicketByPatientAsync(patientId);

        var result = tickets.Select(t => new QueueTicketGetDto(
            t.Id, t.PatientId, t.ServiceId, t.Status, t.ScheduledAt, t.Number
        ));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Tickets for patient", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetByMedicalServiceIdAsync(Guid medicalServiceId)
    {
        var tickets = await _queueTicketRepository.GetTicketsByServiceAsync(medicalServiceId);

        var result = tickets.Select(t => new QueueTicketGetDto(
            t.Id, t.PatientId, t.ServiceId, t.Status, t.ScheduledAt, t.Number
        ));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Tickets for service", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetActiveTicketsAsync()
    {
        var tickets = await _queueTicketRepository.GetActiveTicketsAsync();

        var result = tickets.Select(t => new QueueTicketGetDto(
            t.Id, t.PatientId, t.ServiceId, t.Status, t.ScheduledAt, t.Number
        ));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Active tickets", result, HttpStatusCode.OK);
    }
}
