using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.QueueTicketDTOs;
using Application.Shared;
using Domain.Entities;
using Domain.Enums;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Persistence.Services;

public class QueueTicketService : IQueueTicketService
{
    private readonly IQueueTicketRepository _queueTicketRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public QueueTicketService(
        IQueueTicketRepository queueTicketRepository,
        IPatientRepository patientRepository,
        IDepartmentRepository departmentRepository)
    {
        _queueTicketRepository = queueTicketRepository;
        _patientRepository = patientRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<BaseResponse<QueueTicketGetDto>> GetByIdAsync(Guid id)
    {
        var ticket = await _queueTicketRepository.GetByIdAsync(id);
        if (ticket is null)
            return new BaseResponse<QueueTicketGetDto>("Ticket not found", HttpStatusCode.NotFound);

        var dto = new QueueTicketGetDto(
            ticket.Id,
            ticket.PatientId,
            ticket.ProcedureId,
            ticket.Status,
            ticket.ScheduledAt,
            ticket.Number
        );

        return new BaseResponse<QueueTicketGetDto>("Ticket found", dto, HttpStatusCode.OK);
    }

    //public async Task<BaseResponse<QueueTicketCreateDto>> CreateAsync(QueueTicketCreateDto dto)
    //{
    //    var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
    //    if (patient == null)
    //        return new BaseResponse<QueueTicketCreateDto>("Patient not found", null, HttpStatusCode.NotFound);

    //    // 🔒 Bloklama yoxlaması
    //    if (patient.BlockedUntil.HasValue && patient.BlockedUntil > DateTime.UtcNow)
    //    {
    //        return new BaseResponse<QueueTicketCreateDto>(
    //            $"Patient is blocked until {patient.BlockedUntil.Value:yyyy-MM-dd HH:mm}",
    //            null,
    //            HttpStatusCode.Forbidden
    //        );
    //    }

    //    var service = await _departmentRepository.GetByIdAsync(dto.ProcedureId);
    //    if (service == null)
    //        return new BaseResponse<QueueTicketCreateDto>("Service not found", null, HttpStatusCode.NotFound);

    //    var lastTicket = await _queueTicketRepository.GetLastTicketByServiceAsync(dto.ProcedureId);
    //    DateTime scheduledAt = lastTicket == null
    //        ? DateTime.UtcNow
    //        : lastTicket.ScheduledAt.AddMinutes(30);

    //    var ticket = new QueueTicket
    //    {
    //        Id = Guid.NewGuid(),
    //        PatientId = dto.PatientId,
    //        ProcedureId = dto.ProcedureId,
    //        DoctorId = service.DoctorId,
    //        Number = lastTicket?.Number + 1 ?? 1,
    //        ScheduledAt = scheduledAt,
    //        Status = QueueStatus.Waiting
    //    };

    //    await _queueTicketRepository.AddAsync(ticket);
    //    await _queueTicketRepository.SaveChangeAsync();

    //    // Hangfire task
    //    BackgroundJob.Schedule<IQueueTicketService>(
    //        x => x.ProcessMissedTicketAsync(ticket.Id),
    //        scheduledAt.AddMinutes(30) - DateTime.UtcNow
    //    );

    //    var result = new QueueTicketGetDto(
    //        ticket.Id, ticket.PatientId, ticket.ProcedureId, ticket.Status, ticket.CreatedAt,ticket.Number
    //    );


    //    return new BaseResponse<QueueTicketCreateDto>(
    //        "Ticket created successfully",
    //        HttpStatusCode.OK
    //    );
    //}

    public async Task<BaseResponse<QueueTicketGetDto>> CreateAsync(QueueTicketCreateDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
        if (patient == null)
            return new BaseResponse<QueueTicketGetDto>("Patient not found", null, HttpStatusCode.NotFound);

        // 🔒 Bloklama yoxlaması
        if (patient.BlockedUntil.HasValue && patient.BlockedUntil > DateTime.UtcNow)
        {
            return new BaseResponse<QueueTicketGetDto>(
                $"Patient is blocked until {patient.BlockedUntil.Value:yyyy-MM-dd HH:mm}",
                null,
                HttpStatusCode.Forbidden
            );
        }

        // Service (Procedure əslində Department-dir)
        var service = await _departmentRepository.GetByIdAsync(dto.ProcedureId);
        if (service == null)
            return new BaseResponse<QueueTicketGetDto>("Service not found", null, HttpStatusCode.NotFound);

        // Random həkim seç
        var doctors = service.Doctors.ToList();
        if (!doctors.Any())
            return new BaseResponse<QueueTicketGetDto>("No doctors found in this service", null, HttpStatusCode.NotFound);

        var random = new Random();
        var selectedDoctor = doctors[random.Next(doctors.Count)];

        // Son ticket-i tap (yalnız bu həkim + bu department üzrə)
        var lastTicket = await _queueTicketRepository.GetLastTicketByDoctorAsync(selectedDoctor.Id, service.Id);

        DateTime scheduledAt = lastTicket == null
            ? DateTime.UtcNow
            : lastTicket.ScheduledAt.AddMinutes(30);

        // Yeni ticket yaradılır
        var ticket = new QueueTicket
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            ProcedureId = dto.ProcedureId,
            DoctorId = selectedDoctor.Id,
            Number = lastTicket?.Number + 1 ?? 1,
            ScheduledAt = scheduledAt,
            Status = QueueStatus.Waiting,
            CreatedAt = DateTime.UtcNow
        };

        await _queueTicketRepository.AddAsync(ticket);
        await _queueTicketRepository.SaveChangeAsync();

        // Hangfire task → 30 dəqiqədən sonra missed ticket yoxlamaq üçün
        BackgroundJob.Schedule<IQueueTicketService>(
            x => x.ProcessMissedTicketAsync(ticket.Id),
            scheduledAt.AddMinutes(30) - DateTime.UtcNow
        );

        // İstifadəçiyə qaytarmaq üçün DTO
        var result = new QueueTicketGetDto(
            ticket.Id,
            ticket.PatientId,
            ticket.ProcedureId,
            ticket.Status,
            ticket.CreatedAt,
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
            return new BaseResponse<bool>("Ticket not found", HttpStatusCode.NotFound);

        ticket.Status = dto.NewStatus;
        _queueTicketRepository.Update(ticket);
        await _queueTicketRepository.SaveChangeAsync();

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
            _queueTicketRepository.Update(ticket);
            await _queueTicketRepository.SaveChangeAsync();

            var patient = await _patientRepository.GetByIdAsync(ticket.PatientId);
            if (patient != null)
            {
                patient.MissedTurns += 1;

                if (patient.MissedTurns >= 3)
                {
                    patient.BlockedUntil = DateTime.UtcNow.AddDays(7);
                    patient.MissedTurns = 0;
                }

                _patientRepository.Update(patient);
                await _patientRepository.SaveChangeAsync();
            }
        }
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetByPatientIdAsync(Guid patientId)
    {
        var tickets = await _queueTicketRepository.GetByFiltered(x => x.PatientId == patientId).ToListAsync();

        var result = tickets.Select(t => new QueueTicketGetDto(
            t.Id, t.PatientId, t.ProcedureId, t.Status, t.ScheduledAt, t.Number
        ));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Tickets for patient", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetByProcedureIdAsync(Guid medicalServiceId)
    {
        var tickets = await _queueTicketRepository.GetTicketsByServiceAsync(medicalServiceId);

        var result = tickets.Select(t => new QueueTicketGetDto(
            t.Id, t.PatientId, t.ProcedureId, t.Status, t.ScheduledAt, t.Number
        ));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Tickets for service", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetActiveTicketsAsync()
    {
        var tickets = await _queueTicketRepository.GetByFiltered(
            t => t.Status == QueueStatus.Waiting || t.Status == QueueStatus.Called
        ).ToListAsync();

        var result = tickets.Select(t => new QueueTicketGetDto(
            t.Id, t.PatientId, t.ProcedureId, t.Status, t.ScheduledAt, t.Number
        ));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Active tickets", result, HttpStatusCode.OK);
    }
}
