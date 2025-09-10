using Application.Abstracts.Services;
using Application.DTOs.QueueTicketDTOs;
using Application.Shared;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System.Net;

namespace Persistence.Services;

public class QueueTicketService : IQueueTicketService
{
    private readonly AppDbContext _context;

    public QueueTicketService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<QueueTicketGetDto>> GetByIdAsync(Guid id)
    {
        var ticket = await _context.QueueTickets.FindAsync(id);
        if (ticket is null)
            return new BaseResponse<QueueTicketGetDto>("Ticket not found", HttpStatusCode.NotFound);

        return new BaseResponse<QueueTicketGetDto>("Ticket found", new QueueTicketGetDto(
            ticket.Id, ticket.PatientId, ticket.ServiceId, ticket.Status, ticket.CreatedAt
        ), HttpStatusCode.OK);
    }

    public async Task<BaseResponse<QueueTicketGetDto>> CreateAsync(QueueTicketCreateDto dto)
    {
        var ticket = new QueueTicket
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            ServiceId = dto.MedicalServiceId,
            Status = QueueStatus.Waiting,
            CreatedAt = DateTime.UtcNow
        };

        _context.QueueTickets.Add(ticket);
        await _context.SaveChangesAsync();

        return new BaseResponse<QueueTicketGetDto>("Ticket created", new QueueTicketGetDto(
            ticket.Id, ticket.PatientId, ticket.ServiceId, ticket.Status, ticket.CreatedAt
        ), HttpStatusCode.Created);
    }

    public async Task<BaseResponse<bool>> UpdateStatusAsync(QueueTicketUpdateStatusDto dto)
    {
        var ticket = await _context.QueueTickets.FindAsync(dto.TicketId);
        if (ticket is null)
            return new BaseResponse<bool>("Ticket not found", HttpStatusCode.NotFound);

        ticket.Status = dto.NewStatus;
        await _context.SaveChangesAsync();

        return new BaseResponse<bool>("Status updated", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<bool>> ProcessMissedTicketAsync(Guid ticketId)
    {
        var ticket = await _context.QueueTickets.FindAsync(ticketId);
        if (ticket is null)
            return new BaseResponse<bool>("Ticket not found", HttpStatusCode.NotFound);

        ticket.Status = QueueStatus.Missed;
        await _context.SaveChangesAsync();

        return new BaseResponse<bool>("Ticket marked as missed", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetByPatientIdAsync(Guid patientId)
    {
        var tickets = await _context.QueueTickets
            .Where(t => t.PatientId == patientId)
            .ToListAsync();

        var result = tickets.Select(t => new QueueTicketGetDto(t.Id, t.PatientId, t.ServiceId, t.Status, t.CreatedAt));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Tickets for patient", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetByMedicalServiceIdAsync(Guid medicalServiceId)
    {
        var tickets = await _context.QueueTickets
            .Where(t => t.ServiceId == medicalServiceId)
            .ToListAsync();

        var result = tickets.Select(t => new QueueTicketGetDto(t.Id, t.PatientId, t.ServiceId, t.Status, t.CreatedAt));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Tickets for service", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<IEnumerable<QueueTicketGetDto>>> GetActiveTicketsAsync()
    {
        var tickets = await _context.QueueTickets
            .Where(t => t.Status == QueueStatus.Waiting || t.Status == QueueStatus.Called)
            .ToListAsync();

        var result = tickets.Select(t => new QueueTicketGetDto(t.Id, t.PatientId, t.ServiceId, t.Status, t.CreatedAt));

        return new BaseResponse<IEnumerable<QueueTicketGetDto>>("Active tickets", result, HttpStatusCode.OK);
    }
}
