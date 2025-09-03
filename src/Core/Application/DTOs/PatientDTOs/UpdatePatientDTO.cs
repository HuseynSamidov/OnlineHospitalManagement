using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.PatientDTOs;

public class UpdatePatientDTO
{
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
}
