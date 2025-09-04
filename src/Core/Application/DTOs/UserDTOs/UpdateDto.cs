using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDTOs;

public record UpdateDto(
 string FullName,
 string? Email = null,
 string? PhoneNumber = null,
 string? Password = null
);
