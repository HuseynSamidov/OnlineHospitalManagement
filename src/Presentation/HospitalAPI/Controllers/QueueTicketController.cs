using Application.Abstracts.Services;
using Application.DTOs.QueueTicketDTOs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class QueueTicketController : ControllerBase
{
    private readonly IQueueTicketService _queueTicketService;

    public QueueTicketController(IQueueTicketService queueTicketService)
    {
        _queueTicketService = queueTicketService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] QueueTicketCreateDto dto)
    {
        var result = await _queueTicketService.CreateAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _queueTicketService.GetByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("status")]
    public async Task<IActionResult> UpdateStatusAsync([FromBody] QueueTicketUpdateStatusDto dto)
    {
        var result = await _queueTicketService.UpdateStatusAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatientIdAsync(Guid patientId)
    {
        var result = await _queueTicketService.GetByPatientIdAsync(patientId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("service/{serviceId}")]
    public async Task<IActionResult> GetByMedicalServiceIdAsync(Guid serviceId)
    {
        var result = await _queueTicketService.GetByMedicalServiceIdAsync(serviceId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveTicketsAsync()
    {
        var result = await _queueTicketService.GetActiveTicketsAsync();
        return StatusCode((int)result.StatusCode, result);
    }
}
